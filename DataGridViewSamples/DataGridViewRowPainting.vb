Imports System
Imports System.Drawing
Imports System.Windows.Forms

Class DataGridViewRowPainting
    Inherits Form
    Private WithEvents dataGridView1 As New DataGridView()
    Private oldRowIndex As Int32 = 0
    Private Const CUSTOM_CONTENT_HEIGHT As Int32 = 30

    <STAThreadAttribute()>
    Public Shared Sub Main()

        Application.Run(New DataGridViewRowPainting())

    End Sub 'Main

    Public Sub New()

        Me.dataGridView1.Dock = DockStyle.Fill
        Me.Controls.Add(Me.dataGridView1)
        Me.Text = "DataGridView row painting demo"

    End Sub 'NewNew

    Sub DataGridViewRowPainting_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        ' Set a cell padding to provide space for the top of the focus 
        ' rectangle and for the content that spans multiple columns. 
        Dim newPadding As New Padding(0, 1, 0, CUSTOM_CONTENT_HEIGHT)

        Me.dataGridView1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Top
        Me.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        Me.dataGridView1.MultiSelect = True
        Me.dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText

        Me.dataGridView1.RowTemplate.DefaultCellStyle.Padding = newPadding

        ' Set the selection background color to transparent so 
        ' the cell won't paint over the custom selection background.
        Me.dataGridView1.RowTemplate.DefaultCellStyle.SelectionBackColor = Color.Transparent

        ' Set the row height to accommodate the normal cell content and the 
        ' content that spans multiple columns.
        Me.dataGridView1.RowTemplate.Height += CUSTOM_CONTENT_HEIGHT

        ' Initialize other DataGridView properties.
        Me.dataGridView1.AllowUserToAddRows = False
        Me.dataGridView1.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2
        Me.dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.None
        Me.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect

        ' Set the column header names.
        Me.dataGridView1.ColumnCount = 4
        Me.dataGridView1.Columns(0).Name = "Recipe"
        Me.dataGridView1.Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable
        Me.dataGridView1.Columns(1).Name = "Category"
        Me.dataGridView1.Columns(2).Name = "Main Ingredients"
        Me.dataGridView1.Columns(3).Name = "Rating"

        ' Hide the column that contains the content that spans 
        ' multiple columns.
        Me.dataGridView1.Columns(2).Visible = False

        ' Populate the rows of the DataGridView.
        Dim row1() As String = {"Meatloaf", "Main Dish", "1 lb. lean ground beef, 1/2 cup bread crumbs, 1/4 cup ketchup, 1/3 tsp onion powder, 1 clove of garlic, 1/2 pack onion soup mix, dash of your favorite BBQ Sauce", "****"}
        Dim row2() As String = {"Key Lime Pie", "Dessert", "lime juice, whipped cream, eggs, evaporated milk", "****"}
        Dim row3() As String = {"Orange-Salsa Pork Chops", "Main Dish", "pork chops, salsa, orange juice, pineapple", "****"}
        Dim row4() As String = {"Black Bean and Rice Salad", "Salad", "black beans, brown rice", "****"}
        Dim row5() As String = {"Chocolate Cheesecake", "Dessert", "cream cheese, unsweetened chocolate", "***"}
        Dim row6() As String = {"Black Bean Dip", "Appetizer", "black beans, sour cream, salsa, chips", "***"}
        Dim rows() As Object = {row1, row2, row3, row4, row5, row6}
        Dim rowArray As String()
        For Each rowArray In rows
            Me.dataGridView1.Rows.Add(rowArray)
        Next rowArray

        ' Adjust the row heights to accommodate the normal cell content.
        Me.dataGridView1.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders)
    End Sub

    ' Forces the control to repaint itself when the user 
    ' manually changes the width of a column.
    Sub dataGridView1_ColumnWidthChanged(ByVal sender As Object, ByVal e As DataGridViewColumnEventArgs) Handles dataGridView1.ColumnWidthChanged
        Me.dataGridView1.Invalidate()
    End Sub
    ' Forces the row to repaint itself when the user changes the current cell. This is necessary to refresh the focus rectangle.
    Sub dataGridView1_CurrentCellChanged(ByVal sender As Object, ByVal e As EventArgs) Handles dataGridView1.CurrentCellChanged
        If oldRowIndex <> -1 Then
            Me.dataGridView1.InvalidateRow(oldRowIndex)
        End If
        oldRowIndex = Me.dataGridView1.CurrentCellAddress.Y
    End Sub

    Private Sub dataGridView1_CellPainting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellPaintingEventArgs) Handles dataGridView1.CellPainting

        If Me.dataGridView1.Columns("Rating").Index = e.ColumnIndex AndAlso e.RowIndex >= 0 Then

            Dim newRect As New Rectangle(e.CellBounds.X + 1, e.CellBounds.Y + 1,
            e.CellBounds.Width - 4, e.CellBounds.Height - 4)
            Dim backColorBrush As New SolidBrush(e.CellStyle.BackColor)
            Dim gridBrush As New SolidBrush(Me.dataGridView1.GridColor)
            Dim gridLinePen As New Pen(gridBrush)

            Try
                ' Erase the cell.
                e.Graphics.FillRectangle(backColorBrush, e.CellBounds)

                ' Draw the grid lines (only the right and bottom lines; DataGridView takes care of the others).
                e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                    e.CellBounds.Bottom - 1, e.CellBounds.Right - 1,
                    e.CellBounds.Bottom - 1)
                e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1,
                    e.CellBounds.Top, e.CellBounds.Right - 1,
                    e.CellBounds.Bottom)

                ' Draw the inset highlight box.
                e.Graphics.DrawRectangle(Pens.Blue, newRect)

                ' Draw the text content of the cell, ignoring alignment.
                If (e.Value IsNot Nothing) Then
                    e.Graphics.DrawString(CStr(e.Value), e.CellStyle.Font, Brushes.Crimson, e.CellBounds.X + 2, e.CellBounds.Y + 2, StringFormat.GenericDefault)
                End If
                e.Handled = True
            Finally
                gridLinePen.Dispose()
                gridBrush.Dispose()
                backColorBrush.Dispose()
            End Try
        End If
    End Sub
    ' Paints the custom selection background for selected rows.
    Sub dataGridView1_RowPrePaint(ByVal sender As Object, ByVal e As DataGridViewRowPrePaintEventArgs) Handles dataGridView1.RowPrePaint
        ' Do not automatically paint the focus rectangle.
        e.PaintParts = e.PaintParts And Not DataGridViewPaintParts.Focus

        ' Determine whether the cell should be painted with the custom selection background.
        If (e.State And DataGridViewElementStates.Selected) = DataGridViewElementStates.Selected Then

            ' Calculate the bounds of the row.
            Dim rowBounds As New Rectangle(
                Me.dataGridView1.RowHeadersWidth,
                e.RowBounds.Top,
                Me.dataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - Me.dataGridView1.HorizontalScrollingOffset + 1,
                e.RowBounds.Height)

            ' Paint the custom selection background.
            Dim backbrush As New _
                System.Drawing.Drawing2D.LinearGradientBrush(rowBounds,
                Me.dataGridView1.DefaultCellStyle.SelectionBackColor,
                e.InheritedRowStyle.ForeColor,
                System.Drawing.Drawing2D.LinearGradientMode.Horizontal)
            Try
                e.Graphics.FillRectangle(backbrush, rowBounds)
            Finally
                backbrush.Dispose()
            End Try
        End If

    End Sub
    ' Paints the content that spans multiple columns and the focus rectangle.
    Sub dataGridView1_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles dataGridView1.RowPostPaint

        ' Calculate the bounds of the row.
        Dim rowBounds As New Rectangle(Me.dataGridView1.RowHeadersWidth,
            e.RowBounds.Top,
            Me.dataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - Me.dataGridView1.HorizontalScrollingOffset + 1,
            e.RowBounds.Height)
        Dim forebrush As SolidBrush = Nothing
        Try
            ' Determine the foreground color.
            forebrush = New SolidBrush(IIf((e.State And DataGridViewElementStates.Selected) = DataGridViewElementStates.Selected, e.InheritedRowStyle.SelectionForeColor, e.InheritedRowStyle.ForeColor))

            ' Get the content that spans multiple columns.
            Dim recipe As Object = Me.dataGridView1.Rows.SharedRow(e.RowIndex).Cells(2).Value

            If (recipe IsNot Nothing) Then
                Dim text As String = recipe.ToString()

                ' Calculate the bounds for the content that spans multiple 
                ' columns, adjusting for the horizontal scrolling position 
                ' and the current row height, and displaying only whole
                ' lines of text.
                Dim textArea As Rectangle = rowBounds
                textArea.X -= Me.dataGridView1.HorizontalScrollingOffset
                textArea.Width += Me.dataGridView1.HorizontalScrollingOffset
                textArea.Y += rowBounds.Height - e.InheritedRowStyle.Padding.Bottom
                textArea.Height -= rowBounds.Height - e.InheritedRowStyle.Padding.Bottom
                textArea.Height = (textArea.Height \ e.InheritedRowStyle.Font.Height) * e.InheritedRowStyle.Font.Height

                ' Calculate the portion of the text area that needs painting.
                Dim clip As RectangleF = textArea
                clip.Width -= Me.dataGridView1.RowHeadersWidth + 1 - clip.X
                clip.X = Me.dataGridView1.RowHeadersWidth + 1
                Dim oldClip As RectangleF = e.Graphics.ClipBounds
                e.Graphics.SetClip(clip)

                ' Draw the content that spans multiple columns.
                e.Graphics.DrawString(text, e.InheritedRowStyle.Font, forebrush, textArea)
                e.Graphics.SetClip(oldClip)
            End If
        Finally
            forebrush.Dispose()
        End Try

        If Me.dataGridView1.CurrentCellAddress.Y = e.RowIndex Then
            ' Paint the focus rectangle.
            e.DrawFocus(rowBounds, True)
        End If

    End Sub

    ' Adjusts the padding when the user changes the row height so that 
    ' the normal cell content is fully displayed and any extra
    ' height is used for the content that spans multiple columns.
    Sub dataGridView1_RowHeightChanged(ByVal sender As Object, ByVal e As DataGridViewRowEventArgs) Handles dataGridView1.RowHeightChanged

        ' Calculate the new height of the normal cell content.
        Dim preferredNormalContentHeight As Int32 =
            e.Row.GetPreferredHeight(e.Row.Index,
            DataGridViewAutoSizeRowMode.AllCellsExceptHeader, True) -
            e.Row.DefaultCellStyle.Padding.Bottom()

        ' Specify a new padding.
        Dim newPadding As Padding = e.Row.DefaultCellStyle.Padding
        newPadding.Bottom = e.Row.Height - preferredNormalContentHeight
        e.Row.DefaultCellStyle.Padding = newPadding

    End Sub

    Private Sub dataGridView1_SelectionChanged(sender As Object, e As EventArgs) Handles dataGridView1.SelectionChanged
        Debug.WriteLine("Selections:")
        For Each iRow As DataGridViewRow In Me.dataGridView1.SelectedRows
            Debug.WriteLine(String.Format("Row({0}): {1}{2}", iRow.Index, iRow.ToString, IIf(iRow Is Me.dataGridView1.CurrentRow, "*", "")))
        Next
    End Sub
End Class