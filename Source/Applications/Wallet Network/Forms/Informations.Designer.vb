﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Informations
    Inherits System.Windows.Forms.Form

    'Form esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
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

    'Richiesto da Progettazione Windows Form
    Private components As System.ComponentModel.IContainer

    'NOTA: la procedura che segue è richiesta da Progettazione Windows Form
    'Può essere modificata in Progettazione Windows Form.  
    'Non modificarla mediante l'editor del codice.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.releaseLabel = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.PixBay = New System.Windows.Forms.LinkLabel()
        Me.Close = New System.Windows.Forms.Button()
        Me.IconsLinkLabel = New System.Windows.Forms.LinkLabel()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(84, 29)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(110, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Application name:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(200, 29)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(106, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Wallet Network"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(137, 51)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(57, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Release:"
        '
        'releaseLabel
        '
        Me.releaseLabel.AutoSize = True
        Me.releaseLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.releaseLabel.Location = New System.Drawing.Point(200, 51)
        Me.releaseLabel.Name = "releaseLabel"
        Me.releaseLabel.Size = New System.Drawing.Size(27, 13)
        Me.releaseLabel.TabIndex = 3
        Me.releaseLabel.Text = "0.1"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(123, 73)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(71, 13)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Thank's to:"
        '
        'PixBay
        '
        Me.PixBay.AutoSize = True
        Me.PixBay.Location = New System.Drawing.Point(200, 73)
        Me.PixBay.Name = "PixBay"
        Me.PixBay.Size = New System.Drawing.Size(111, 13)
        Me.PixBay.TabIndex = 5
        Me.PixBay.TabStop = True
        Me.PixBay.Text = "Background photo"
        '
        'Close
        '
        Me.Close.Location = New System.Drawing.Point(164, 132)
        Me.Close.Name = "Close"
        Me.Close.Size = New System.Drawing.Size(75, 23)
        Me.Close.TabIndex = 6
        Me.Close.Text = "Close"
        Me.Close.UseVisualStyleBackColor = True
        '
        'IconsLinkLabel
        '
        Me.IconsLinkLabel.AutoSize = True
        Me.IconsLinkLabel.Location = New System.Drawing.Point(200, 96)
        Me.IconsLinkLabel.Name = "IconsLinkLabel"
        Me.IconsLinkLabel.Size = New System.Drawing.Size(73, 13)
        Me.IconsLinkLabel.TabIndex = 7
        Me.IconsLinkLabel.TabStop = True
        Me.IconsLinkLabel.Text = "Icon's Back"
        '
        'Informations
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(417, 167)
        Me.ControlBox = False
        Me.Controls.Add(Me.IconsLinkLabel)
        Me.Controls.Add(Me.Close)
        Me.Controls.Add(Me.PixBay)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.releaseLabel)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MaximumSize = New System.Drawing.Size(433, 206)
        Me.MinimumSize = New System.Drawing.Size(433, 206)
        Me.Name = "Informations"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Informations"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents releaseLabel As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents PixBay As LinkLabel
    Friend WithEvents Close As Button
    Friend WithEvents IconsLinkLabel As LinkLabel
End Class
