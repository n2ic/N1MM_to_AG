Imports System.Net
Imports System.Xml
Imports System.Net.Sockets
Imports System.IO
Imports System.Text.RegularExpressions
Public Class Form1

    Public n1mmUDP As UdpClient
    Public AGIPAddress As String
    Dim oldradioname(2) As String
    Public lastmessage(2) As String
    Dim sendingtcp As Boolean = False
    Public tcpsendtimer As System.Timers.Timer

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim s() As String = System.Environment.GetCommandLineArgs()

        oldradioname(1) = ""
        oldradioname(2) = ""
        lastmessage(1) = ""
        lastmessage(2) = ""
        'tcpsendtimer = New Timers.Timer(5000)
        tcpsendtimer = New Timers.Timer(1000)
        AddHandler tcpsendtimer.Elapsed, New Timers.ElapsedEventHandler(AddressOf TimerElapsed)

        If s.Length > 1 Then
            AGIPAddress = s(1)
            Label1.Text = AGIPAddress
            Label1.Visible = True
        Else
            MsgBox("AG IP Address needed as command line argument")
            End
        End If
    End Sub

    Private Sub N1MMtoAG()
        n1mmUDP = New UdpClient(13090)
        Try
            n1mmUDP.BeginReceive(AddressOf HandleUDPMessage, n1mmUDP)

        Catch ex As SocketException
            If ex.ErrorCode = 10048 Then
                MsgBox("Receiving UDP N1MMSocket 13090 is already in use", MsgBoxStyle.Exclamation, "N1MMSocket error")
            End If
        End Try
    End Sub

    Private Sub HandleUDPMessage(state As IAsyncResult)
        Dim receiveBytes() As Byte = {}
        Dim RemoteIpEndPoint As New IPEndPoint(IPAddress.Any, 0)
        Dim cmd As String = ""
        Dim radioNr As Int16
        Dim freq As Int32
        Dim index As Integer
        Dim radioname As String = ""

        If n1mmUDP Is Nothing Then Exit Sub
        If n1mmUDP.Client Is Nothing Then Exit Sub
        Try  ' don't care if the object is disposed.
            receiveBytes = n1mmUDP.EndReceive(state, RemoteIpEndPoint)
        Catch e As ObjectDisposedException
            Exit Sub
        End Try
        Dim temp As String = System.Text.Encoding.ASCII.GetString(receiveBytes)
        Using reader = XmlReader.Create(New StringReader(temp), New XmlReaderSettings With {.CheckCharacters = False})
            Try
                While reader.Read()
                    Select Case reader.NodeType
                        Case XmlNodeType.Element
                            cmd = reader.Name
                    End Select
                    If cmd <> "" Then
                        Exit While
                    End If
                End While

                Select Case cmd
                    Case "RadioInfo"
                        While reader.Read
                            If reader.IsStartElement Then
                                Select Case reader.Name
                                    Case "RadioNr"
                                        reader.Read()
                                        radioNr = Convert.ToInt16(CleanInvalidXmlChars(reader.Value))
                                    Case "Freq"
                                        reader.Read()
                                        freq = Convert.ToInt32(CleanInvalidXmlChars(reader.Value))
                                    Case "RadioName"
                                        reader.Read()
                                        radioname = CleanInvalidXmlChars(reader.Value)
                                End Select
                            End If
                        End While
                End Select
            Catch ex As Xml.XmlException
                MsgBox("XmlException")
            End Try
        End Using

        If radioname <> oldradioname(radioNr) Then
            If radioname = "Manual" Then
                MsgBox("Lost communication with Radio " & radioNr & ". Continuing to send last received band to AG", MsgBoxStyle.SystemModal, "Lost radio comms")
            End If
        End If

        oldradioname(radioNr) = radioname

        If freq = 0 Then 'ignore - N1MM starting up
            n1mmUDP.BeginReceive(AddressOf HandleUDPMessage, n1mmUDP)
            Exit Sub
        End If

        freq = freq \ 100000

        Select Case freq
            Case 1
                index = 1
            Case 3
                index = 2
            Case 7
                index = 3
            Case 10
                index = 4
            Case 14
                index = 5
            Case 18
                index = 6
            Case 21
                index = 7
            Case 24
                index = 8
            Case 28, 29
                index = 9
            Case Else
                MsgBox("Invalid frequency received from Radio " & radioNr & ". ", MsgBoxStyle.SystemModal, "Invalid frequency")
                n1mmUDP.BeginReceive(AddressOf HandleUDPMessage, n1mmUDP)
                Exit Sub
        End Select

        Dim lastmsg As String
        lastmsg = "!000a!00cc80!" & radioNr.ToString & ";" & index.ToString

        If radioNr = 1 Then
            lastmessage(1) = lastmsg
        Else
            lastmessage(2) = lastmsg
        End If

        If Not sendingtcp Then
            tcpsendtimer.Stop()
            SendTCP(lastmsg)
            tcpsendtimer.Enabled = True
        End If

        n1mmUDP.BeginReceive(AddressOf HandleUDPMessage, n1mmUDP)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If sender.Text.ToString = "Start" Then
            tcpsendtimer.Enabled = True
            N1MMtoAG()
            Button1.Text = "Stop"
        Else
            End
        End If
    End Sub

    Public Function CleanInvalidXmlChars(valueOrAttribute As String) As String
        Static re As Regex = New Regex("[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]")
        Return re.Replace(valueOrAttribute, "")
    End Function

    Private Sub SendTCP(message As String)
        Dim port As Int32 = 9007
        Dim client As TcpClient
        Dim lostcomm As New LostComms

        sendingtcp = True
        ' Translate the passed message into ASCII and store it as a Byte array. 
        Dim data As [Byte]() = System.Text.Encoding.ASCII.GetBytes(message)

        Try
            'client = New TcpClient(AGIPAddress, port)
            client = New TcpClient()
            If Not My.Computer.Network.Ping(AGIPAddress, 1000) Then
                Debug.Print("Timeout 1000")
                If Not My.Computer.Network.Ping(AGIPAddress, 50) Then
                    lostcomm.ShowDialog()
                    Exit Sub
                End If
            End If
            If Not client.ConnectAsync(AGIPAddress, port).Wait(4000) Then
                lostcomm.ShowDialog()
                Exit Sub
            End If

            ' Get a client stream for reading and writing. 
            Dim stream As NetworkStream = client.GetStream()

            ' Send the message to the connected TcpServer. 
            stream.Write(data, 0, data.Length)
            stream.Close()
            client.Close()
        Catch ex As Exception
            lostcomm.ShowDialog()
        End Try
        sendingtcp = False
    End Sub

    Private Sub TimerElapsed(sender As Object, e As EventArgs)
        tcpsendtimer.Stop()
        If sendingtcp Then
            tcpsendtimer.Enabled = True
            Exit Sub
        End If
        If lastmessage(1) <> "" Then
            SendTCP(lastmessage(1))
        End If
        If lastmessage(2) <> "" Then
            SendTCP(lastmessage(2))
        End If
        tcpsendtimer.Enabled = True
    End Sub

    'Private Sub Button2_Click(sender As Object, e As EventArgs)
    '    SendTCP("!000a!00cc80!1;9")
    'End Sub
End Class
