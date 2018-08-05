Imports System
Imports System.Windows.Forms

Public Class CalendarColumn
    Inherits DataGridViewColumn
    Public Sub New()
        MyBase.New(New CalendarCell())
    End Sub
    Public Overrides Property CellTemplate() As DataGridViewCell
        Get
            Return MyBase.CellTemplate
        End Get
        Set(ByVal value As DataGridViewCell)
            ' Ensure that the cell used for the template is a CalendarCell.
            If (value IsNot Nothing) AndAlso Not value.GetType().IsAssignableFrom(GetType(CalendarCell)) Then Throw New InvalidCastException("Must be a CalendarCell")
            MyBase.CellTemplate = value
        End Set
    End Property
End Class

Public Class CalendarCell
    Inherits DataGridViewTextBoxCell
    Public Sub New()
        ' Use the short date format.
        Me.Style.Format = "d"
    End Sub
    Public Overrides Sub InitializeEditingControl(ByVal rowIndex As Integer, ByVal initialFormattedValue As Object, ByVal dataGridViewCellStyle As DataGridViewCellStyle)
        ' Set the value of the editing control to the current cell value.
        MyBase.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle)
        Dim ctl As CalendarEditingControl = CType(DataGridView.EditingControl, CalendarEditingControl)

        ' Use the default row value when Value property is null.
        If (Me.Value Is Nothing) Then
            ctl.Value = CType(Me.DefaultNewRowValue, DateTime)
        Else
            ctl.Value = CType(Me.Value, DateTime)
        End If
    End Sub
    Public Overrides ReadOnly Property EditType() As Type
        Get
            ' Return the type of the editing control that CalendarCell uses.
            Return GetType(CalendarEditingControl)
        End Get
    End Property
    Public Overrides ReadOnly Property ValueType() As Type
        Get
            ' Return the type of the value that CalendarCell contains.
            Return GetType(DateTime)
        End Get
    End Property
    Public Overrides ReadOnly Property DefaultNewRowValue() As Object
        Get
            ' Use the current date and time as the default value.
            Return DateTime.Now
        End Get
    End Property
End Class

Class CalendarEditingControl
    Inherits DateTimePicker
    Implements IDataGridViewEditingControl

    Private dataGridViewControl As DataGridView
    Private valueIsChanged As Boolean = False
    Private rowIndexNum As Integer

    Public Sub New()
        Me.Format = DateTimePickerFormat.Short
    End Sub

    Public Property EditingControlFormattedValue() As Object Implements IDataGridViewEditingControl.EditingControlFormattedValue
        Get
            Return Me.Value.ToShortDateString()
        End Get
        Set(ByVal value As Object)
            Try
                ' This will throw an exception of the string is 
                ' null, empty, or not in the format of a date.
                Me.Value = DateTime.Parse(CStr(value))
            Catch
                ' In the case of an exception, just use the default
                ' value so we're not left with a null value.
                Me.Value = DateTime.Now
            End Try
        End Set
    End Property
    Public Function GetEditingControlFormattedValue(ByVal context As DataGridViewDataErrorContexts) As Object Implements IDataGridViewEditingControl.GetEditingControlFormattedValue
        Return Me.Value.ToShortDateString()
    End Function
    Public Sub ApplyCellStyleToEditingControl(ByVal dataGridViewCellStyle As DataGridViewCellStyle) Implements IDataGridViewEditingControl.ApplyCellStyleToEditingControl
        Me.Font = dataGridViewCellStyle.Font
        Me.CalendarForeColor = dataGridViewCellStyle.ForeColor
        Me.CalendarMonthBackground = dataGridViewCellStyle.BackColor
    End Sub
    Public Property EditingControlRowIndex() As Integer Implements IDataGridViewEditingControl.EditingControlRowIndex
        Get
            Return rowIndexNum
        End Get
        Set(ByVal value As Integer)
            rowIndexNum = value
        End Set
    End Property
    Public Function EditingControlWantsInputKey(ByVal key As Keys, ByVal dataGridViewWantsInputKey As Boolean) As Boolean Implements IDataGridViewEditingControl.EditingControlWantsInputKey
        ' Let the DateTimePicker handle the keys listed.
        Select Case key And Keys.KeyCode
            Case Keys.Left, Keys.Up, Keys.Down, Keys.Right,
                Keys.Home, Keys.End, Keys.PageDown, Keys.PageUp
                Return True
            Case Else
                Return Not dataGridViewWantsInputKey
        End Select
    End Function
    Public Sub PrepareEditingControlForEdit(ByVal selectAll As Boolean) Implements IDataGridViewEditingControl.PrepareEditingControlForEdit
        ' No preparation needs to be done.
    End Sub
    Public ReadOnly Property RepositionEditingControlOnValueChange() As Boolean Implements IDataGridViewEditingControl.RepositionEditingControlOnValueChange
        Get
            Return False
        End Get
    End Property
    Public Property EditingControlDataGridView() As DataGridView Implements IDataGridViewEditingControl.EditingControlDataGridView
        Get
            Return dataGridViewControl
        End Get
        Set(ByVal value As DataGridView)
            dataGridViewControl = value
        End Set
    End Property
    Public Property EditingControlValueChanged() As Boolean Implements IDataGridViewEditingControl.EditingControlValueChanged
        Get
            Return valueIsChanged
        End Get
        Set(ByVal value As Boolean)
            valueIsChanged = value
        End Set
    End Property
    Public ReadOnly Property EditingControlCursor() As Cursor Implements IDataGridViewEditingControl.EditingPanelCursor
        Get
            Return MyBase.Cursor
        End Get
    End Property
    Protected Overrides Sub OnValueChanged(ByVal eventargs As EventArgs)
        ' Notify the DataGridView that the contents of the cell have changed.
        valueIsChanged = True
        Me.EditingControlDataGridView.NotifyCurrentCellDirty(True)
        MyBase.OnValueChanged(eventargs)
    End Sub
End Class

Public Class Form1
    Inherits Form

    Private dataGridView1 As New DataGridView()

    <STAThreadAttribute()>
    Public Shared Sub Main()
        Application.Run(New Form1())
    End Sub
    Public Sub New()
        Me.dataGridView1.Dock = DockStyle.Fill
        Me.Controls.Add(Me.dataGridView1)
        Me.Text = "DataGridView calendar column demo"
    End Sub
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim col As New CalendarColumn()
        Me.dataGridView1.Columns.Add(col)
        Me.dataGridView1.RowCount = 5
        Dim row As DataGridViewRow
        For Each row In Me.dataGridView1.Rows
            row.Cells(0).Value = DateTime.Now
        Next row
    End Sub
End Class