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
        TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.0F))
        TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67.0F))
        TableLayoutPanel.Controls.Add(LabelProductName, 1, 0)
        TableLayoutPanel.Controls.Add(LabelVersion, 1, 1)
        TableLayoutPanel.Controls.Add(LabelCopyright, 1, 2)
        TableLayoutPanel.Controls.Add(LabelCompanyName, 1, 3)
        TableLayoutPanel.Controls.Add(TextBoxDescription, 1, 4)
        TableLayoutPanel.Controls.Add(OKButton, 1, 5)
        TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        TableLayoutPanel.Location = New System.Drawing.Point(12, 14)
        TableLayoutPanel.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        TableLayoutPanel.Name = "TableLayoutPanel"
        TableLayoutPanel.RowCount = 6
        TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0F))
        TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0F))
        TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0F))
        TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0F))
        TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0F))
        TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0F))
        TableLayoutPanel.Size = New System.Drawing.Size(431, 594)
        TableLayoutPanel.TabIndex = 0
        ' 
        ' LabelProductName
        ' 
        LabelProductName.Dock = System.Windows.Forms.DockStyle.Fill
        LabelProductName.Location = New System.Drawing.Point(8, 0)
        LabelProductName.Margin = New System.Windows.Forms.Padding(8, 0, 4, 0)
        LabelProductName.MaximumSize = New System.Drawing.Size(0, 26)
        LabelProductName.Name = "LabelProductName"
        LabelProductName.Size = New System.Drawing.Size(419, 26)
        LabelProductName.TabIndex = 0
        LabelProductName.Text = "Product Name"
        LabelProductName.TextAlign = Drawing.ContentAlignment.MiddleLeft
        ' 
        ' LabelVersion
        ' 
        LabelVersion.Dock = System.Windows.Forms.DockStyle.Fill
        LabelVersion.Location = New System.Drawing.Point(8, 59)
        LabelVersion.Margin = New System.Windows.Forms.Padding(8, 0, 4, 0)
        LabelVersion.MaximumSize = New System.Drawing.Size(0, 26)
        LabelVersion.Name = "LabelVersion"
        LabelVersion.Size = New System.Drawing.Size(419, 26)
        LabelVersion.TabIndex = 0
        LabelVersion.Text = "Version"
        LabelVersion.TextAlign = Drawing.ContentAlignment.MiddleLeft
        ' 
        ' LabelCopyright
        ' 
        LabelCopyright.Dock = System.Windows.Forms.DockStyle.Fill
        LabelCopyright.Location = New System.Drawing.Point(8, 118)
        LabelCopyright.Margin = New System.Windows.Forms.Padding(8, 0, 4, 0)
        LabelCopyright.MaximumSize = New System.Drawing.Size(0, 26)
        LabelCopyright.Name = "LabelCopyright"
        LabelCopyright.Size = New System.Drawing.Size(419, 26)
        LabelCopyright.TabIndex = 0
        LabelCopyright.Text = "Copyright"
        LabelCopyright.TextAlign = Drawing.ContentAlignment.MiddleLeft
        ' 
        ' LabelCompanyName
        ' 
        LabelCompanyName.Dock = System.Windows.Forms.DockStyle.Fill
        LabelCompanyName.Location = New System.Drawing.Point(8, 177)
        LabelCompanyName.Margin = New System.Windows.Forms.Padding(8, 0, 4, 0)
        LabelCompanyName.MaximumSize = New System.Drawing.Size(0, 26)
        LabelCompanyName.Name = "LabelCompanyName"
        LabelCompanyName.Size = New System.Drawing.Size(419, 26)
        LabelCompanyName.TabIndex = 0
        LabelCompanyName.Text = "Company Name"
        LabelCompanyName.TextAlign = Drawing.ContentAlignment.MiddleLeft
        ' 
        ' TextBoxDescription
        ' 
        TextBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill
        TextBoxDescription.Location = New System.Drawing.Point(8, 241)
        TextBoxDescription.Margin = New System.Windows.Forms.Padding(8, 5, 4, 5)
        TextBoxDescription.Multiline = True
        TextBoxDescription.Name = "TextBoxDescription"
        TextBoxDescription.ReadOnly = True
        TextBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both
        TextBoxDescription.Size = New System.Drawing.Size(419, 287)
        TextBoxDescription.TabIndex = 0
        TextBoxDescription.TabStop = False
        TextBoxDescription.Text = resources.GetString("TextBoxDescription.Text")
        ' 
        ' OKButton
        ' 
        OKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right
        OKButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        OKButton.Location = New System.Drawing.Point(327, 554)
        OKButton.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        OKButton.Name = "OKButton"
        OKButton.Size = New System.Drawing.Size(100, 35)
        OKButton.TabIndex = 0
        OKButton.Text = "&OK"
        ' 
        ' frmAboutDFLOW
        ' 
        AutoScaleDimensions = New System.Drawing.SizeF(8F, 20F)
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        CancelButton = OKButton
        ClientSize = New System.Drawing.Size(455, 622)
        Controls.Add(TableLayoutPanel)
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        MaximizeBox = False
        MinimizeBox = False
        Name = "frmAboutDFLOW"
        Padding = New System.Windows.Forms.Padding(12, 14, 12, 14)
        ShowInTaskbar = False
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Text = "About DFLOW"
        TableLayoutPanel.ResumeLayout(False)
        TableLayoutPanel.PerformLayout()
        ResumeLayout(False)
    End Sub

End Class
