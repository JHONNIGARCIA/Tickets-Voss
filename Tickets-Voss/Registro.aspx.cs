using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Web.UI;

namespace Tickets_Voss
{
    public partial class Registro : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioCorreo"] == null || !(Session["EsAdmin"] != null && (bool)Session["EsAdmin"]))
            {
                Response.Redirect("IT.aspx");
            }
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(password))
            {
                MostrarAlerta("Por favor completa todos los campos.", "warning");
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ConexionTickets"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Validar si el correo ya existe
                string checkQuery = "SELECT COUNT(*) FROM Usuarios WHERE Correo = @Correo";
                SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@Correo", correo);

                int count = (int)checkCommand.ExecuteScalar();

                if (count > 0)
                {
                    MostrarAlerta("El correo ya está registrado.", "error");
                    return;
                }

                // Insertar el nuevo usuario
                string insertQuery = "INSERT INTO Usuarios (Nombre, Correo, Password, EsAdmin) VALUES (@Nombre, @Correo, @Password, 0)";
                SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                insertCommand.Parameters.AddWithValue("@Nombre", nombre);
                insertCommand.Parameters.AddWithValue("@Correo", correo);
                insertCommand.Parameters.AddWithValue("@Password", password);


                insertCommand.ExecuteNonQuery();

                MostrarAlerta("Usuario registrado exitosamente.", "success");

                // Limpiar campos
                txtNombre.Text = "";
                txtCorreo.Text = "";
                txtPassword.Text = "";
            }
        }

        private void MostrarAlerta(string mensaje, string tipo)
        {
            string script = $@"Swal.fire({{
                                position: 'top-end',
                                icon: '{tipo}',
                                title: '{mensaje}',
                                showConfirmButton: false,
                                timer: 2000
                            }});";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "SweetAlert", script, true);
        }
    }
}
