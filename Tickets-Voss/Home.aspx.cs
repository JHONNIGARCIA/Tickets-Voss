using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tickets_Voss
{
    public partial class Home : System.Web.UI.Page
    {
        private static int TicketSeleccionadoId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioCorreo"] == null)
            {
                Response.Redirect("IT.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LlenarMeses();
                CargarResumenPorMesSeleccionado();
                CargarResumen();
                CargarTickets();
            }
        }

        private void LlenarMeses()
        {
            ddlMes.Items.Clear();
            ddlMes.Items.Add(new ListItem("Todos", "0"));
            for (int i = 1; i <= 12; i++)
            {
                string nombreMes = new DateTime(2025, i, 1).ToString("MMMM");
                ddlMes.Items.Add(new ListItem(nombreMes, i.ToString()));
            }
            ddlMes.SelectedValue = DateTime.Now.Month.ToString();
        }

        private void CargarResumenPorMesSeleccionado()
        {
            int mes = int.Parse(ddlMes.SelectedValue);
            DataTable resumen = ObtenerResumenPorMes(mes);

            gvResumenMensual.DataSource = resumen;
            gvResumenMensual.DataBind();

            // Actualizar cards
            lblTotalUsuarios.Text = ObtenerDato("SELECT COUNT(*) FROM Usuarios").ToString();
            lblTotalSolicitudes.Text = ObtenerDato($"SELECT COUNT(*) FROM Solicitudes {(mes > 0 ? "WHERE MONTH(FechaRegistro)=" + mes : "")}").ToString();
            lblSolicitudesAsignadas.Text = ObtenerDato($"SELECT COUNT(*) FROM Solicitudes WHERE Estatus = 'Asignado' {(mes > 0 ? "AND MONTH(FechaRegistro)=" + mes : "")}").ToString();
            lblSolicitudesCerradas.Text = ObtenerDato($"SELECT COUNT(*) FROM Solicitudes WHERE Estatus = 'Cerrado' {(mes > 0 ? "AND MONTH(FechaRegistro)=" + mes : "")}").ToString();
        }

        protected void ddlMes_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarResumenPorMesSeleccionado();
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            // Prepara el contenido para exportar
            DataTable dt = ObtenerTicketsParaExportar();

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=Tickets_Solicitudes.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table border='1'>");

            // Agregar los encabezados de la tabla
            sb.Append("<tr>");
            foreach (DataColumn column in dt.Columns)
            {
                sb.Append("<th>" + column.ColumnName + "</th>");
            }
            sb.Append("</tr>");

            // Agregar las filas de la tabla
            foreach (DataRow row in dt.Rows)
            {
                sb.Append("<tr>");
                foreach (var item in row.ItemArray)
                {
                    sb.Append("<td>" + item.ToString() + "</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }
        private DataTable ObtenerTicketsParaExportar()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConexionTickets"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT 
                            Id,
                            Nombre,
                            Apellido,
                            Departamento,
                            Telefono,
                            Correo,
                            Categoria,
                            Descripcion,
                            Estatus,
                            FechaRegistro,
                            FechaFin,
                            AsignadoA
                        FROM Solicitudes";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }


        private DataTable ObtenerResumenPorMes(int mes)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConexionTickets"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        DATENAME(MONTH, FechaRegistro) AS Mes,
                        COUNT(*) AS Total
                    FROM Solicitudes
                    WHERE (@mes = 0 OR MONTH(FechaRegistro) = @mes)
                    GROUP BY DATENAME(MONTH, FechaRegistro), MONTH(FechaRegistro)
                    ORDER BY MONTH(FechaRegistro)";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@mes", mes);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private object ObtenerDato(string query)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConexionTickets"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                return cmd.ExecuteScalar();
            }
        }


        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.RawUrl);
        }

        private void CargarResumen()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConexionTickets"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Total usuarios
                SqlCommand cmdUsuarios = new SqlCommand("SELECT COUNT(*) FROM Usuarios", conn);
                lblTotalUsuarios.Text = cmdUsuarios.ExecuteScalar().ToString();

                // Total solicitudes
                SqlCommand cmdTotal = new SqlCommand("SELECT COUNT(*) FROM Solicitudes", conn);
                lblTotalSolicitudes.Text = cmdTotal.ExecuteScalar().ToString();

                // Asignadas (estatus = Asignado o Recibido)
                SqlCommand cmdAsignadas = new SqlCommand("SELECT COUNT(*) FROM Solicitudes WHERE Estatus = 'Asignado'", conn);
                lblSolicitudesAsignadas.Text = cmdAsignadas.ExecuteScalar().ToString();

                // Cerradas
                SqlCommand cmdCerradas = new SqlCommand("SELECT COUNT(*) FROM Solicitudes WHERE Estatus = 'Cerrado'", conn);
                lblSolicitudesCerradas.Text = cmdCerradas.ExecuteScalar().ToString();
            }
        }


        private void CargarTickets(string filtro = "")
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConexionTickets"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT Id, Nombre, Apellido, Departamento, Telefono, Correo, Categoria, Descripcion, Estatus, FechaRegistro, FechaFin, AsignadoA
                    FROM Solicitudes
                    WHERE 
                        (@filtro = '' OR 
                         Id LIKE '%' + @filtro + '%' OR 
                         Nombre LIKE '%' + @filtro + '%' OR 
                         Apellido LIKE '%' + @filtro + '%' OR 
                         Departamento LIKE '%' + @filtro + '%' OR 
                         Categoria LIKE '%' + @filtro + '%' OR 
                         Descripcion LIKE '%' + @filtro + '%' OR 
                         Correo LIKE '%' + @filtro + '%')
                    ORDER BY FechaRegistro DESC";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@filtro", filtro);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvTickets.DataSource = dt;
                gvTickets.DataBind();
            }
        }

        protected void gvTickets_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditarTicket")
            {
                int ticketId = Convert.ToInt32(e.CommandArgument);
                TicketSeleccionadoId = ticketId;

                string connectionString = ConfigurationManager.ConnectionStrings["ConexionTickets"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Solicitudes WHERE Id = @Id", conn);
                    cmd.Parameters.AddWithValue("@Id", ticketId);

                    string asignadoActual = "";

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblNombre.Text = reader["Nombre"].ToString();
                        lblApellido.Text = reader["Apellido"].ToString();
                        lblDepartamento.Text = reader["Departamento"].ToString();
                        descripcionContainer.InnerText = reader["Descripcion"].ToString();
                        ddlEstatus.SelectedValue = reader["Estatus"].ToString();
                        asignadoActual = reader["AsignadoA"].ToString();
                    }
                    reader.Close();

                    // Cargar usuarios
                    cmd = new SqlCommand("SELECT Id, Nombre FROM Usuarios", conn);
                    reader = cmd.ExecuteReader();
                    ddlAsignadoA.Items.Clear();
                    while (reader.Read())
                    {
                        string nombre = reader["Nombre"].ToString();
                        ddlAsignadoA.Items.Add(new ListItem(nombre, nombre));
                    }
                    reader.Close();

                    // Seleccionar asignado actual si está en la lista
                    if (ddlAsignadoA.Items.FindByValue(asignadoActual) != null)
                    {
                        ddlAsignadoA.SelectedValue = asignadoActual;
                    }
                }

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowModal", "$('#modalAsignar').modal('show');", true);
            }
        }

 

        protected void btnGuardarAsignacion_Click(object sender, EventArgs e)
        {
            if (TicketSeleccionadoId == 0) return;

            string asignadoA = ddlAsignadoA.SelectedValue;
            string estatus = ddlEstatus.SelectedValue;

            if (string.IsNullOrWhiteSpace(asignadoA) || string.IsNullOrWhiteSpace(estatus)) return;

            string connectionString = ConfigurationManager.ConnectionStrings["ConexionTickets"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    UPDATE Solicitudes 
                    SET AsignadoA = @AsignadoA, 
                        Estatus = @Estatus, 
                        FechaFin = CASE WHEN @Estatus = 'Cerrado' THEN GETDATE() ELSE FechaFin END 
                    WHERE Id = @Id";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AsignadoA", asignadoA);
                command.Parameters.AddWithValue("@Estatus", estatus);
                command.Parameters.AddWithValue("@Id", TicketSeleccionadoId);

                connection.Open();
                command.ExecuteNonQuery();
            }

            TicketSeleccionadoId = 0;
            CargarTickets();

            ScriptManager.RegisterStartupScript(this, this.GetType(), "SweetAlertSuccess", @"
                Swal.fire({
                    icon: 'success',
                    title: '¡Éxito!',
                    text: 'Status del Ticket correctamente actuliazado.',
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 2000,
                    toast: true
                });
            ", true);
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("IT.aspx");
        }

        protected void btnAsignar_Click(object sender, EventArgs e)
        {
            int ticketId = Convert.ToInt32((sender as Button).CommandArgument);
            TicketSeleccionadoId = ticketId;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "abrirModal", "$('#modalAsignar').modal('show');", true);
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string texto = txtBuscar.Text.Trim();
            CargarTickets(texto);
        }
    }
}
