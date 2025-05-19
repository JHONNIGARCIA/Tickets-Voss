using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Net;
using System.Net.Mail;

namespace Tickets_Voss
{
    public partial class IT : Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.Cookies["Usuario"] != null && Request.Cookies["Password"] != null)
                {
                    txtCorreo.Text = Request.Cookies["Usuario"].Value;
                    txtPassword.Attributes["value"] = Request.Cookies["Password"].Value;
                    chkRecordar.Checked = true;
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string correo = txtCorreo.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Validar campos vacíos
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(password))
            {
                MostrarAlerta("Por favor, completa todos los campos.", "warning");
                return;
            }

            if (ValidarCredenciales(correo, password, out bool esAdmin))
            {
                Session["UsuarioCorreo"] = correo;
                Session["EsAdmin"] = esAdmin;

                // Si el checkbox está marcado, guardar cookies
                if (chkRecordar.Checked)
                {
                    Response.Cookies["Usuario"].Value = correo;
                    Response.Cookies["Usuario"].Expires = DateTime.Now.AddDays(30);

                    Response.Cookies["Password"].Value = password;
                    Response.Cookies["Password"].Expires = DateTime.Now.AddDays(30);
                }
                else
                {
                    // Eliminar cookies si no está marcado
                    Response.Cookies["Usuario"].Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies["Password"].Expires = DateTime.Now.AddDays(-1);
                }

                // Redirigir
                if (esAdmin)
                    Response.Redirect("Registro.aspx");
                else
                    Response.Redirect("Home.aspx");
            }
            else
            {
                MostrarAlerta("Correo o contraseña incorrectos.", "error");
            }
        }


        private bool ValidarCredenciales(string correo, string password, out bool esAdmin)
        {
            esAdmin = false;

            if (correo.Equals("IT2025@voss.net", StringComparison.OrdinalIgnoreCase) && password == "LagunaIT")
            {
                esAdmin = true;
                return true;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ConexionTickets"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT EsAdmin FROM Usuarios WHERE Correo = @Correo AND Password = @Password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Correo", correo);
                command.Parameters.AddWithValue("@Password", password);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    esAdmin = Convert.ToBoolean(reader["EsAdmin"]);
                    return true;
                }
            }

            return false;
        }

        private void MostrarAlerta(string mensaje, string tipo)
        {
            string script = $@"Swal.fire({{
                position: 'top-end',
                icon: '{tipo}',
                title: '{mensaje}',
                showConfirmButton: false,
                timer: 2000,
                toast: true
            }});";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "SweetAlert", script, true);
        }
    }
}
