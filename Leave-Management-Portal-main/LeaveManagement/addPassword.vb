﻿Public Class addPassword

    Public setPassword As Boolean = False

    'a function that verifies if there is any upper case alphabet in the entered password
    Function ContainsUpperCase(ByVal inputString As String) As Boolean
        'iterating through whole input string entered
        For Each character As Char In inputString
            If Char.IsUpper(character) Then
                Return True
            End If
        Next

        Return False
    End Function

    'a function that verifies if there is any lower case alphabet in the entered password
    Function ContainsLowerCase(ByVal inputString As String) As Boolean
        'iterating through whole input string entered
        For Each character As Char In inputString
            If Char.IsLower(character) Then
                Return True
            End If
        Next

        Return False
    End Function

    'a function that verifies if there is any digit(0-9) in the entered password
    Function ContainsDigit(ByVal inputString As String) As Boolean
        'iterating through whole input string entered
        For Each character As Char In inputString
            If Char.IsDigit(character) Then
                Return True
            End If
        Next

        Return False
    End Function

    'a function that verifies if there is any special character in the entered password
    Function ContainsSpecialCharacter(ByVal inputString As String) As Boolean
        'iterating through whole input string entered
        For Each character As Char In inputString
            If Not Char.IsLetterOrDigit(character) Then
                ' If the character is not a letter or digit, consider it as a special character
                Return True
            End If
        Next

        Return False
    End Function

    'password strength check
    Private Function PasswordStrengthCheck(ByVal InputString As String) As Integer
        Dim checkVar As Integer = 0
        Dim message As String = "Password Strength Check:" & vbCrLf

        ' Checking length criteria
        If InputString.Length >= 8 Then
            checkVar += 1
            message &= "✓ Length should be at least 8 characters." & vbCrLf
        Else
            message &= "✗ Length should be at least 8 characters." & vbCrLf
        End If

        ' Checking if the password has a digit
        If ContainsDigit(InputString) Then
            checkVar += 1
            message &= "✓ Contains at least one digit." & vbCrLf
        Else
            message &= "✗ Should contain at least one digit." & vbCrLf
        End If

        ' Checking if the password has a lowercase letter
        If ContainsLowerCase(InputString) Then
            checkVar += 1
            message &= "✓ Contains at least one lowercase letter." & vbCrLf
        Else
            message &= "✗ Should contain at least one lowercase letter." & vbCrLf
        End If

        ' Checking if the password has a special character
        If ContainsSpecialCharacter(InputString) Then
            checkVar += 1
            message &= "✓ Contains at least one special character." & vbCrLf
        Else
            message &= "✗ Should contain at least one special character." & vbCrLf
        End If

        ' Checking if the password has an uppercase letter
        If ContainsUpperCase(InputString) Then
            checkVar += 1
            message &= "✓ Contains at least one uppercase letter." & vbCrLf
        Else
            message &= "✗ Should contain at least one uppercase letter." & vbCrLf
        End If

        If checkVar < 5 Then
            MessageBox.Show(message, "Password Strength", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        Return checkVar

    End Function

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim userEmail = Environment.GetEnvironmentVariable("userEmail")
        Dim role = Environment.GetEnvironmentVariable("role")

        If TextBox1.Text <> TextBox2.Text Then
            MessageBox.Show("Passwords do no match", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim inputString As String = TextBox1.Text

        If inputString.Length < 1 Then
            MessageBox.Show("Please enter a non empty password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim StrengthCheck As Integer = PasswordStrengthCheck(inputString)

        If StrengthCheck < 5 Then
            Return
        End If

        Using connection As New MySqlConnection(My.Settings.connectionString)
            Try
                connection.Open()

                Dim query As String = "INSERT INTO authentication (email, passwd, role) VALUES (@email, SHA2(CONCAT(@email, @password), 256), @role)"

                Dim password As String = inputString

                Dim command As New MySqlCommand(query, connection)
                command.Parameters.AddWithValue("@email", userEmail)
                command.Parameters.AddWithValue("@password", password)
                command.Parameters.AddWithValue("@role", role)

                command.ExecuteNonQuery()

            Catch ex As MySqlException
                connection.Close()
                MessageBox.Show("Error: " & ex.Message)
            Finally
                MessageBox.Show("Password added!")
                setPassword = True
            End Try
        End Using
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        MessageBox.Show("For stronger password please ensure that it has atleast 8 characters and to include atleast one uppercase letter, one lower case letter, one digit and one special character")
    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        TextBox1.UseSystemPasswordChar = CheckBox1.Checked
        TextBox2.UseSystemPasswordChar = CheckBox1.Checked
    End Sub

    Private Sub addPassword_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        If setPassword Then
            Application.Exit()
        Else
            MessageBox.Show("please set your password!")
            Return
        End If
    End Sub

    Private Sub addPassword_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class