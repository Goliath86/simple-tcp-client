Imports System.Net.Sockets

Public Class Form1

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'Dim buff() As Byte = New Byte() {0, 2, 3, 4, 5} ' The first byte is ignored, the second is the ASCII code for STX (Start Of Text)
        Dim buff() As Byte = New Byte() {67, 105, 97, 111}
        Dim message() As Byte
        message = sendMessage(buff)
        TextBox5.ResetText()
        TextBox5.Text = UnicodeBytesToString(message)
    End Sub

    Private Function UnicodeStringToBytes(ByVal str As String) As Byte()
        Return System.Text.Encoding.Unicode.GetBytes(str)
    End Function

    Private Function UnicodeBytesToString(ByVal byt() As Byte) As String
        Return System.Text.Encoding.Default.GetString(byt)
    End Function

    Private Function CheckAddress() As Boolean
        If TextBox1.Text.Trim() = "" Or TextBox2.Text.Trim() = "" Or TextBox3.Text.Trim() = "" Or TextBox4.Text.Trim = "" Then
            MessageBox.Show("The IP address fields are not all correctly compiled. The IP address must be in the form: XXX.XXX.XXX.XXX where XXX are a number between 0 and 255")
            Return False
        End If
        Return True
    End Function

    Private Function sendMessage(messageBytes As Byte()) As Byte()
        ' Check if a valid IP address is inserted
        If Not CheckAddress() Then Exit Function

        Const bytesize As Integer = 1024 * 1024
        Try
            ' Reconstruct the IP address as string
            Dim ipAddress As String = TextBox1.Text + "." + TextBox2.Text + "." + TextBox3.Text + "." + TextBox4.Text

            ' Try connecting and send the message bytes  
            Dim client As New TcpClient(ipAddress, 11100)

            ' Create a new connection  
            Dim stream As NetworkStream = client.GetStream()

            stream.Write(messageBytes, 0, messageBytes.Length)

            ' Clear the message
            messageBytes = New Byte(bytesize - 1) {}

            ' Receive the stream of bytes  
            stream.Read(messageBytes, 0, messageBytes.Length)

            ' If I've correctly received the end of transmission byte EOT (ASCII char 4)
            For Each character As Byte In messageBytes
                If character = 4 Then
                    stream.Close()
                    'stream.Dispose()
                    client.Close()
                End If
            Next

        Catch e As Exception
            ' Catch exceptions  
            MsgBox(e.Message)
        End Try

        Return messageBytes
        ' Return response  
    End Function

    Private Sub TextBoxes_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox1.KeyPress, TextBox2.KeyPress, TextBox3.KeyPress, TextBox4.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                If Asc(e.KeyChar) = 46 Then
                    SendKeys.Send("{TAB}")
                End If
                e.Handled = True
            End If
        End If
    End Sub
End Class
