<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LostComms
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.OKbutton = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'OKbutton
        '
        Me.OKbutton.Location = New System.Drawing.Point(294, 167)
        Me.OKbutton.Name = "OKbutton"
        Me.OKbutton.Size = New System.Drawing.Size(75, 23)
        Me.OKbutton.TabIndex = 1
        Me.OKbutton.Text = "OK"
        Me.OKbutton.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 27.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Red
        Me.Label1.Location = New System.Drawing.Point(108, 72)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(568, 42)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "WARNING - Lost Comms to AG"
        '
        'LostComms
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(749, 234)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.OKbutton)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "LostComms"
        Me.Text = "LostComms"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents OKbutton As Button
    Friend WithEvents Label1 As Label
End Class
