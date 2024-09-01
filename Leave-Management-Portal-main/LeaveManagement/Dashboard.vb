﻿Public Class Dashboard

    Private Sub Dashboard_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        load_dashboard()
    End Sub

    Public Sub load_dashboard()
        Dim UserEmail As String = Environment.GetEnvironmentVariable("userEmail")

        Using connection As New MySqlConnection(My.Settings.connectionString)
            Try
                connection.Open()

                Dim query As String = ""
                Dim role As String = Environment.GetEnvironmentVariable("role")

                If role = "Faculty" Then
                    query = "SELECT * FROM faculty WHERE email = @email"
                ElseIf role = "Staff" Then
                    query = "SELECT * FROM staff WHERE email = @email"
                Else
                    query = "SELECT * FROM students WHERE email = @email"
                End If

                Dim command As New MySqlCommand(query, connection)
                command.Parameters.AddWithValue("@email", UserEmail)

                Dim casual As Integer
                Dim medical As Integer
                Dim academic As Integer
                Dim on_duty As Integer
                Dim maternal As Integer

                Dim reader As MySqlDataReader = command.ExecuteReader()
                If reader.Read() Then
                    casual = reader.GetUInt32("casual")
                    medical = reader.GetUInt32("medical")
                    academic = reader.GetUInt32("academic")
                    on_duty = reader.GetUInt32("on_duty")
                    maternal = reader.GetUInt32("maternity")
                End If
                reader.Close()
                DataGridView1.AllowUserToAddRows = False
                DataGridView1.RowHeadersVisible = False
                DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                DataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells

                DataGridView1.ScrollBars = ScrollBars.Vertical
                Dim newRowValues() As String = {casual.ToString, academic.ToString, medical.ToString, on_duty.ToString, maternal.ToString}
                DataGridView1.Rows.Add(newRowValues)

            Catch ex As MySqlException
                connection.Close()
                MessageBox.Show("Error: " & ex.Message)
            End Try
        End Using

        Using connection As New MySqlConnection(My.Settings.connectionString)
            Try

                Dim query As String = "SELECT application_id as 'ID', type as Nature, from_date as 'From Date' , to_date as 'To Date', reason as Reason, status as Status FROM requests WHERE applicant_email = @email AND status = 'pending'"

                Dim command As New MySqlCommand(query, connection)
                command.Parameters.AddWithValue("@email", UserEmail)

                Dim dataAdapter As New MySqlDataAdapter(command)

                Dim dataTable As New DataTable
                dataAdapter.Fill(dataTable)
                data_active_requests.DataSource = dataTable
                If dataTable.Rows.Count = 0 Then
                    data_active_requests.Visible = False
                    Label2.Visible = True
                Else
                    Label2.Visible = False
                    ' Display the fetched data in the GroupBox
                    data_active_requests.DataSource = dataTable
                    'DisplayRequestsInGroupBox(dataTable)
                    data_active_requests.AllowUserToAddRows = False
                    data_active_requests.RowHeadersVisible = False
                    data_active_requests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                    data_active_requests.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
                    data_active_requests.ScrollBars = ScrollBars.Vertical
                    Dim newColumn As New DataGridViewButtonColumn()
                    newColumn.Name = "Cancel Leave"
                    newColumn.UseColumnTextForButtonValue = True
                    newColumn.Text = "Cancel"
                    newColumn.DefaultCellStyle.BackColor = Color.Crimson
                    newColumn.DefaultCellStyle.SelectionBackColor = Color.Crimson
                    newColumn.DefaultCellStyle.ForeColor = Color.White
                    newColumn.DefaultCellStyle.SelectionForeColor = Color.White
                    newColumn.FlatStyle = FlatStyle.Flat
                    data_active_requests.Columns.Add(newColumn)
                End If

            Catch ex As MySqlException
                MessageBox.Show("Error: " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub Panel1_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel1.Paint

    End Sub

    Private Sub data_active_request_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles data_active_requests.CellContentClick
        If e.RowIndex < 0 Then
            Return
        End If

        Dim row As DataGridViewRow = data_active_requests.Rows(e.RowIndex)
        RequestDetails.application_id = CInt(row.Cells("application_id").Value.ToString())
        RequestDetails.Button3.Visible = True
        Dim role As String = Environment.GetEnvironmentVariable("role")
        If role = "Faculty" Then
            ' Dim RequestDetails As RequestDetails = New RequestDetails()
            ' faculty.switchPanel(RequestDetails)
            RequestDetails.Show()
        Else
            ' student.switchPanel(RequestDetails)
            RequestDetails.Show()
        End If
    End Sub


End Class