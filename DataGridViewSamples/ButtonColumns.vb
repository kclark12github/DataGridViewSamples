Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Windows.Forms.VisualStyles
Public Class ButtonColumns
    Inherits Form
    Private WithEvents dataGridView1 As New DataGridView()
    <STAThread()>
    Public Shared Sub Main()
        Application.EnableVisualStyles()
        Application.Run(New Form1())
    End Sub
    Public Sub New()
        Me.AutoSize = True
    End Sub
    Public Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim column0 As New DataGridViewCheckBoxColumn()
        Dim column1 As New DataGridViewDisableButtonColumn()
        column0.Name = "CheckBoxes"
        column1.Name = "Buttons"
        dataGridView1.Columns.Add(column0)
        dataGridView1.Columns.Add(column1)

        dataGridView1.RowCount = 8
        dataGridView1.AutoSize = True
        dataGridView1.AllowUserToAddRows = False
        dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        dataGridView1.AllowUserToOrderColumns = True

        ' Set the text for each button.
        Dim i As Integer
        For i = 0 To dataGridView1.RowCount - 1
            dataGridView1.Rows(i).Cells("Buttons").Value = "Button " + i.ToString()
        Next i
        Me.Controls.Add(dataGridView1)
    End Sub

    ' This event handler manually raises the CellValueChanged event
    ' by calling the CommitEdit method.
    Sub dataGridView1_CurrentCellDirtyStateChanged(ByVal sender As Object, ByVal e As EventArgs) Handles dataGridView1.CurrentCellDirtyStateChanged
        If dataGridView1.IsCurrentCellDirty Then
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    ' If a check box cell is clicked, this event handler disables  
    ' or enables the button in the same row as the clicked cell.
    Public Sub dataGridView1_CellValueChanged(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles dataGridView1.CellValueChanged
        If dataGridView1.Columns(e.ColumnIndex).Name = "CheckBoxes" Then
            Dim buttonCell As DataGridViewDisableButtonCell = CType(dataGridView1.Rows(e.RowIndex).Cells("Buttons"), DataGridViewDisableButtonCell)
            Dim checkCell As DataGridViewCheckBoxCell = CType(dataGridView1.Rows(e.RowIndex).Cells("CheckBoxes"), DataGridViewCheckBoxCell)
            buttonCell.Enabled = Not CType(checkCell.Value, [Boolean])

            dataGridView1.Invalidate()
        End If
    End Sub

    ' If the user clicks on an enabled button cell, this event handler  
    ' reports that the button is enabled.
    Sub dataGridView1_CellClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles dataGridView1.CellClick
        If dataGridView1.Columns(e.ColumnIndex).Name = "Buttons" Then
            Dim buttonCell As DataGridViewDisableButtonCell = CType(dataGridView1.Rows(e.RowIndex).Cells("Buttons"), DataGridViewDisableButtonCell)
            If buttonCell.Enabled Then
                MsgBox(dataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString() + " is enabled")
            End If
        End If
    End Sub
End Class
Public Class DataGridViewDisableButtonColumn
    Inherits DataGridViewButtonColumn
    Public Sub New()
        Me.CellTemplate = New DataGridViewDisableButtonCell()
    End Sub
End Class
Public Class DataGridViewDisableButtonCell
    Inherits DataGridViewButtonCell
    Private enabledValue As Boolean
    Public Property Enabled() As Boolean
        Get
            Return enabledValue
        End Get
        Set(ByVal value As Boolean)
            enabledValue = value
        End Set
    End Property

    ' Override the Clone method so that the Enabled property is copied.
    Public Overrides Function Clone() As Object
        Dim Cell As DataGridViewDisableButtonCell = CType(MyBase.Clone(), DataGridViewDisableButtonCell)
        Cell.Enabled = Me.Enabled
        Return Cell
    End Function

    ' By default, enable the button cell.
    Public Sub New()
        Me.enabledValue = True
    End Sub

    Protected Overrides Sub Paint(ByVal graphics As Graphics,
        ByVal clipBounds As Rectangle, ByVal cellBounds As Rectangle,
        ByVal rowIndex As Integer,
        ByVal elementState As DataGridViewElementStates,
        ByVal value As Object, ByVal formattedValue As Object,
        ByVal errorText As String,
        ByVal cellStyle As DataGridViewCellStyle,
        ByVal advancedBorderStyle As DataGridViewAdvancedBorderStyle,
        ByVal paintParts As DataGridViewPaintParts)

        ' The button cell is disabled, so paint the border,  
        ' background, and disabled button for the cell.
        If Not Me.enabledValue Then

            ' Draw the background of the cell, if specified.
            If (paintParts And DataGridViewPaintParts.Background) = DataGridViewPaintParts.Background Then
                Dim cellBackground As New SolidBrush(cellStyle.BackColor)
                graphics.FillRectangle(cellBackground, cellBounds)
                cellBackground.Dispose()
            End If

            ' Draw the cell borders, if specified.
            If (paintParts And DataGridViewPaintParts.Border) = DataGridViewPaintParts.Border Then
                PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle)
            End If

            ' Calculate the area in which to draw the button.
            Dim buttonArea As Rectangle = cellBounds
            Dim buttonAdjustment As Rectangle = Me.BorderWidths(advancedBorderStyle)
            buttonArea.X += buttonAdjustment.X
            buttonArea.Y += buttonAdjustment.Y
            buttonArea.Height -= buttonAdjustment.Height
            buttonArea.Width -= buttonAdjustment.Width

            ' Draw the disabled button.                
            ButtonRenderer.DrawButton(graphics, buttonArea, PushButtonState.Disabled)

            ' Draw the disabled button text. 
            If TypeOf Me.FormattedValue Is String Then
                TextRenderer.DrawText(graphics, CStr(Me.FormattedValue), Me.DataGridView.Font, buttonArea, SystemColors.GrayText)
            End If
        Else
            ' The button cell is enabled, so let the base class 
            ' handle the painting.
            MyBase.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts)
        End If
    End Sub
End Class
