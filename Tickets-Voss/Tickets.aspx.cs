using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tickets_Voss.Models;
using static Tickets_Voss.Models.NotificationHelper;

namespace Tickets_Voss
{
    public partial class Tickets : BasePage
    {
        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar que el correo termine en @voss.net
                if (!txtCorreo.Text.EndsWith("@voss.net", StringComparison.OrdinalIgnoreCase))
                {
                    Alert("El correo debe terminar en @voss.net", NotificationType.error);
                    return;
                }

                // Validar que los campos requeridos no estén vacíos
                if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtApellido.Text) ||
                    string.IsNullOrWhiteSpace(ddlDepartamento.SelectedValue) ||
                    string.IsNullOrWhiteSpace(txtTelefono.Text) ||
                    string.IsNullOrWhiteSpace(txtCorreo.Text) ||
                    string.IsNullOrWhiteSpace(ddlCategoria.SelectedValue) ||
                    string.IsNullOrWhiteSpace(txtDescripcion.Text))
                {
                    Alert("Todos los campos son obligatorios.", NotificationType.warning);
                    return;
                }

                // Validar archivo subido 
                string archivoNombre = fileUpload.HasFile ? fileUpload.FileName : "";
                if (fileUpload.HasFile && string.IsNullOrWhiteSpace(archivoNombre))
                {
                    Alert("El archivo subido no es válido.", NotificationType.error);
                    return;
                }

                if (fileUpload.HasFile)
                {
                    // Tamaño máximo permitido en bytes (4 MB = 4 * 1024 * 1024)
                    int maxSize = 4 * 1024 * 1024;

                    if (fileUpload.PostedFile.ContentLength > maxSize)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "SweetAlertFileSize", @"
                     Swal.fire({
                     icon: 'error',
                     title: 'Archivo demasiado grande',
                     text: 'El archivo no debe exceder los 4 MB',
                     position: 'top-end',
                     showConfirmButton: false,
                     timer: 3000,
                     toast: true
                      });
                       ", true);

                        return;
                    }
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ConexionTickets"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Solicitudes 
                (Nombre, Apellido, Departamento, Telefono, Correo, Categoria, Descripcion, ArchivoNombre, Estatus)
                VALUES 
                (@Nombre, @Apellido, @Departamento, @Telefono, @Correo, @Categoria, @Descripcion, @ArchivoNombre, @Estatus)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", txtNombre.Text);
                        command.Parameters.AddWithValue("@Apellido", txtApellido.Text);
                        command.Parameters.AddWithValue("@Departamento", ddlDepartamento.SelectedValue);
                        command.Parameters.AddWithValue("@Telefono", txtTelefono.Text);
                        command.Parameters.AddWithValue("@Correo", txtCorreo.Text);
                        command.Parameters.AddWithValue("@Categoria", ddlCategoria.SelectedValue);
                        command.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text);
                        command.Parameters.AddWithValue("@ArchivoNombre", archivoNombre);
                        command.Parameters.AddWithValue("@Estatus", "Recibido"); // <-- Se asigna automáticamente

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                // Genera un folio único para el ticket
                string ticketId = "Folio" + DateTime.Now.Ticks.ToString();

                // Enviar correo después de guardar el ticket
                EnviarCorreoTicket(
                    txtCorreo.Text,
                    txtNombre.Text,
                    txtApellido.Text,
                    ticketId,
                    txtDescripcion.Text,
                    ddlDepartamento.SelectedItem.Text,
                    ddlCategoria.SelectedItem.Text,
                    archivoNombre
                );

                Alert("Tu solicitud fue registrada correctamente.", NotificationType.success);

                // ✅ Limpiar campos después de la alerta
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ClearFields", $@"
            document.getElementById('{txtNombre.ClientID}').value = '';
            document.getElementById('{txtApellido.ClientID}').value = '';
            document.getElementById('{txtTelefono.ClientID}').value = '';
            document.getElementById('{txtCorreo.ClientID}').value = '';
            document.getElementById('{txtDescripcion.ClientID}').value = '';
            document.getElementById('{ddlDepartamento.ClientID}').selectedIndex = 0;
            document.getElementById('{ddlCategoria.ClientID}').selectedIndex = 0;
        ", true);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Alert("Ocurrió un error al registrar tu solicitud. Por favor, intenta nuevamente.", NotificationType.error);
                // Opcional: Log del error (puedes implementar un sistema de logging aquí)
                Console.WriteLine(ex.Message);
            }
        }

        private void EnviarCorreoTicket(string correoUsuario, string nombre, string apellido, string ticketId, string descripcion, string departamento, string categoria, string archivoNombre)
        {
            try
            {
                string smtpHost = "smtp.voss.net";
                string smtpCorreo = "IT-VAM-Ticket@voss.net";
                string smtpPassword = "password"; // Cambia por tu contraseña real
                int smtpPort = 25;

                SmtpClient smtp = new SmtpClient
                {
                    Host = smtpHost,
                    Port = smtpPort,
                    EnableSsl = false,
                    Credentials = new NetworkCredential(smtpCorreo, smtpPassword)
                };

                using (MailMessage correo = new MailMessage())
                {
                    correo.From = new MailAddress(smtpCorreo);
                    correo.To.Add(correoUsuario);

                    // CC: todos los usuarios de la tabla Usuarios
                    string connectionString = ConfigurationManager.ConnectionStrings["ConexionTickets"].ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "SELECT Correo FROM Usuarios";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            string correoCC = reader["Correo"].ToString();
                            if (!string.IsNullOrWhiteSpace(correoCC) && !correo.CC.Contains(new MailAddress(correoCC)))
                                correo.CC.Add(correoCC);
                        }
                        reader.Close();
                    }

                    correo.Subject = $"Nueva Solicitud de Servicio - Folio: {ticketId}";
                    correo.Body = $@"
            <h3>Saludos,</h3>
            <p>Se ha creado una nueva solicitud con el folio <strong>{ticketId}</strong>.</p>
            <p><strong>Nombre:</strong> {nombre} {apellido}</p>
            <p><strong>Departamento:</strong> {departamento}</p>
            <p><strong>Categoría:</strong> {categoria}</p>
            <p><strong>Descripción:</strong> {descripcion}</p>
            <p><strong>Estado:</strong> Recibido</p>
            <p>El personal de IT dará seguimiento y lo mantendrá informado.</p>
            <p>Gracias,<br/><strong>Equipo de Soporte IT</strong></p>";
                    correo.IsBodyHtml = true;
                    correo.Priority = MailPriority.Normal;

                    // Adjuntar archivo si existe
                    if (!string.IsNullOrEmpty(archivoNombre) && fileUpload.HasFile)
                    {
                        correo.Attachments.Add(new Attachment(fileUpload.PostedFile.InputStream, archivoNombre));
                    }

                    smtp.Send(correo);
                }
            }
            catch (Exception ex)
            {
                Alert("Error al enviar correo: " + ex.Message, NotificationType.error);
            }
        }
    }
}
