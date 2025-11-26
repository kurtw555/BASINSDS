<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAboutDFLOW
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    Friend WithEvents TableLayoutPanel As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents LabelProductName As System.Windows.Forms.Label
    Friend WithEvents LabelVersion As System.Windows.Forms.Label
    Friend WithEvents LabelCompanyName As System.Windows.Forms.Label
    Friend WithEvents TextBoxDescription As System.Windows.Forms.TextBox
    Friend WithEvents OKButton As System.Windows.Forms.Button
    Friend WithEvents LabelCopyright As System.Windows.Forms.Label

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAboutDFLOW))
        TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        LabelProductName = New System.Windows.Forms.Label()
        LabelVersion = New System.Windows.Forms.Label()
        LabelCopyright = New System.Windows.Forms.Label()
        LabelCompanyName = New System.Windows.Forms.Label()
        TextBoxDescription = New System.Windows.Forms.TextBox()
        OKButton = New System.Windows.Forms.Button()
        TableLayoutPanel.SuspendLayout()
        SuspendLayout()
        ' 
        ' TableLayoutPanel
        ' 
        TableLayoutPanel.ColumnCount = 1
        TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F))
        TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67F))
        TableLayoutPanel.Controls.Add(LabelProductName, 1, 0)
        TableLayoutPanel.Controls.Add(LabelVersion, 1, 1)
        TableLayoutPanel.Controls.Add(LabelCopyright, 1, 2)
        TableLayoutPanel.Controls.Add(LabelCompanyName, 1, 3)
        TableLayoutPanel.Controls.Add(TextBoxDescription, 1, 4)
        TableLayoutPanel.Controls.Add(OKButton, 1, 5)
        TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        TableLayoutPanel.Location = New System.Drawing.Point(10, 10)
        TableLayoutPanel.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        TableLayoutPanel.Name = "TableLayoutPanel"
        TableLayoutPanel.RowCount = 6
        TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F))
        TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F))
        TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F))
        TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F))
        TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F))
        TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F))
        TableLayoutPanel.Size = New System.Drawing.Size(378, 446)
        TableLayoutPanel.TabIndex = 0
        ' 
        ' LabelProductName
        ' 
        LabelProductName.Dock = System.Windows.Forms.DockStyle.Fill
        LabelProductName.Location = New System.Drawing.Point(7, 0)
        LabelProductName.Margin = New System.Windows.Forms.Padding(7, 0, 4, 0)
        LabelProductName.MaximumSize = New System.Drawing.Size(0, 20)
        LabelProductName.Name = "LabelProductName"
        LabelProductName.Size = New System.Drawing.Size(367, 20)
        LabelProductName.TabIndex = 0
        LabelProductName.Text = "Product Name"
        LabelProductName.TextAlign = Drawing.ContentAlignment.MiddleLeft
        ' 
        ' LabelVersion
        ' 
        LabelVersion.Dock = System.Windows.Forms.DockStyle.Fill
        LabelVersion.Location = New System.Drawing.Point(7, 44)
        LabelVersion.Margin = New System.Windows.Forms.Padding(7, 0, 4, 0)
        LabelVersion.MaximumSize = New System.Drawing.Size(0, 20)
        LabelVersion.Name = "LabelVersion"
        LabelVersion.Size = New System.Drawing.Size(367, 20)
        LabelVersion.TabIndex = 0
        LabelVersion.Text = "Version"
        LabelVersion.TextAlign = Drawing.ContentAlignment.MiddleLeft
        ' 
        ' LabelCopyright
        ' 
        LabelCopyright.Dock = System.Windows.Forms.DockStyle.Fill
        LabelCopyright.Location = New System.Drawing.Point(7, 88)
        LabelCopyright.Margin = New System.Windows.Forms.Padding(7, 0, 4, 0)
        LabelCopyright.MaximumSize = New System.Drawing.Size(0, 20)
        LabelCopyright.Name = "LabelCopyright"
        LabelCopyright.Size = New System.Drawing.Size(367, 20)
        LabelCopyright.TabIndex = 0
        LabelCopyright.Text = "Copyright"
        LabelCopyright.TextAlign = Drawing.ContentAlignment.MiddleLeft
        ' 
        ' LabelCompanyName
        ' 
        LabelCompanyName.Dock = System.Windows.Forms.DockStyle.Fill
        LabelCompanyName.Location = New System.Drawing.Point(7, 132)
        LabelCompanyName.Margin = New System.Windows.Forms.Padding(7, 0, 4, 0)
        LabelCompanyName.MaximumSize = New System.Drawing.Size(0, 20)
        LabelCompanyName.Name = "LabelCompanyName"
        LabelCompanyName.Size = New System.Drawing.Size(367, 20)
        LabelCompanyName.TabIndex = 0
        LabelCompanyName.Text = "Company Name"
        LabelCompanyName.TextAlign = Drawing.ContentAlignment.MiddleLeft
        ' 
        ' TextBoxDescription
        ' 
        TextBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill
        TextBoxDescription.Location = New System.Drawing.Point(7, 179)
        TextBoxDescription.Margin = New System.Windows.Forms.Padding(7, 3, 4, 3)
        TextBoxDescription.Multiline = True
        TextBoxDescription.Name = "TextBoxDescription"
        TextBoxDescription.ReadOnly = True
        TextBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both
        TextBoxDescription.Size = New System.Drawing.Size(367, 217)
        TextBoxDescription.TabIndex = 0
        TextBoxDescription.TabStop = False
        TextBoxDescription.Text = resources.GetString("TextBoxDescription.Text")
        ' 
        ' OKButton
        ' 
        OKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right
        OKButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        OKButton.Location = New System.Drawing.Point(286, 416)
        OKButton.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        OKButton.Name = "OKButton"
        OKButton.Size = New System.Drawing.Size(88, 27)
        OKButton.TabIndex = 0
        OKButton.Text = "&OK"
        ' 
        ' frmAboutDFLOW
        ' 
        AutoScaleDimensions = New System.Drawing.SizeF(7F, 15F)
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        CancelButton = OKButton
        ClientSize = New System.Drawing.Size(398, 466)
        Controls.Add(TableLayoutPanel)
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        MaximizeBox = False
        MinimizeBox = False
        Name = "frmAboutDFLOW"
        Padding = New System.Windows.Forms.Padding(10)
        ShowInTaskbar = False
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Text = "About DFLOW"
        TableLayoutPanel.ResumeLayout(False)
        TableLayoutPanel.PerformLayout()
        ResumeLayout(False)

    End Sub

End Class
