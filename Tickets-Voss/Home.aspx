<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Tickets_Voss.Home" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reporte - Tickets IT Voss-Laguna</title>
    <meta http-equiv="refresh" content="180">

    <!-- Fuentes y librerías -->
     <link href="Content/sweetalert2.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;500;700&display=swap" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.7.0.min.js"></script>
   <!-- Bootstrap 4 (modal funciona con jQuery como antes) -->
   <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" />
   <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css" rel="stylesheet" />
    <script src="Scripts/sweetalert2.js"></script>

    <style>
        body {
            background: #f3f4f6;
            font-family: 'Arial Rounded MT', sans-serif;
        }

        .header-bar {
            background-color: white;
            padding: 1rem 2rem;
            border-radius: 15px;
            margin: 2rem auto;
            display: flex;
            justify-content: space-between;
            align-items: center;
            max-width: 1280px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
        }

        .btn-cerrar {
            background-color: #ec4899;
            color: white;
            font-weight: 500;
            border: none;
            padding: 0.5rem 1rem;
            border-radius: 10px;
        }

        .form-card {
            background-color: white;
            border-radius: 20px;
            padding: 2rem;
            box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
            overflow-x: auto;
            max-width: 1690px;
            margin: auto;
        }

        .table-custom {
            width: 100%;
            border-collapse: collapse;
            border: 1px solid #d1d5db;
        }

        .table-custom th, .table-custom td {
            padding: 10px;
            border: 1px solid #d1d5db;
            text-align: center;
            white-space: nowrap;
            font-size: 14px;
        }
 

        .estatus-recibido { color: green; font-weight: bold; }
        .estatus-enproceso { color: orange; font-weight: bold; }
        .estatus-cerrado { color: red; font-weight: bold; }


        .descripcion-columna {
            max-width: 200px;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
        }

        .btn-primary {
            background-color: #3b82f6;
            border: none;
            padding: 6px 12px;
            font-size: 13px;
            font-weight: 500;
            border-radius: 6px;
        }

        .btn-primary:hover {
            background-color: #2563eb;
        }

        .descripcion-scroll {
            max-height: 100px;
            overflow-y: auto;
            background-color: #f9f9f9;
            padding: 8px;
            border-radius: 5px;
            font-size: 14px;
            line-height: 1.4;
            word-wrap: break-word;
        }

        .scrollable-table-container {
          max-height: 500px; /* Ajusta la altura según lo que necesites */
          overflow-y: auto;
          overflow-x: auto;
          border: 1px solid #ccc;
          border-radius: 12px;
        }

        h4 {
            text-align: center;
            font-weight: 600;
            color: #111827;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="header-bar">
            <img src="Content/images/Voss-logo.svg" alt="Logo Voss" style="max-width:150px;" />
            <asp:Button ID="btnCerrarSesion" runat="server" CssClass="btn-cerrar" Text="Cerrar Sesión" OnClick="btnCerrarSesion_Click" />
        </div>
        <!-- Cards de resumen -->
<div class="container mb-4">
    <div class="row text-center">
        <div class="col-md-3">
            <div class="card shadow-sm p-3">
                <img src="Content/images/user.jpg" alt="Usuarios" class="img-fluid" style="height:130px"; />
                <h6 class="mt-2">Usuarios</h6>
                <asp:Label ID="lblTotalUsuarios" runat="server" CssClass="fw-bold fs-5 text-dark" />
            </div>
        </div>
        <div class="col-md-3">
            <div class="card shadow-sm p-3">
                <img src="Content/images/soliciud.png" alt="Solicitudes" class="img-fluid" style="height:130px;" />
                <h6 class="mt-2">Solicitudes</h6>
                <asp:Label ID="lblTotalSolicitudes" runat="server" CssClass="fw-bold fs-5 text-dark" />
            </div>
        </div>
        <div class="col-md-3">
            <div class="card shadow-sm p-3">
                <img src="Content/images/recibido.png" alt="Asignadas" class="img-fluid" style="height:130px;" />
                <h6 class="mt-2">Solicitudes Asignadas</h6>
                <asp:Label ID="lblSolicitudesAsignadas" runat="server" CssClass="fw-bold fs-5 text-dark" />
            </div>
        </div>
        <div class="col-md-3">
            <div class="card shadow-sm p-3">
                <img src="Content/images/terminada.png" alt="Cerradas" class="img-fluid" style="height:130px;" />
                <h6 class="mt-2">Solicitudes Cerradas</h6>
                <asp:Label ID="lblSolicitudesCerradas" runat="server" CssClass="fw-bold fs-5 text-dark" />
            </div>
        </div>
    </div>
</div>
        <div class="">
           <div class="d-flex align-items-center gap-2 mb-3">
    <asp:DropDownList ID="ddlMes" runat="server" AutoPostBack="true" 
        OnSelectedIndexChanged="ddlMes_SelectedIndexChanged" 
        CssClass="form-select me-2" Style="width: 160px;">
    </asp:DropDownList>

    <asp:Button ID="btnExportarExcel" runat="server" CssClass="btn btn-success" 
        Text="Descargar Excel" OnClick="btnExportarExcel_Click" />
</div>


<!-- Agrega el GridView real para contener el resumen del mes (oculto visualmente si solo es para exportar) -->
<asp:GridView ID="gvResumenMensual" runat="server" CssClass="table table-bordered table-sm d-none" />


            <h4 class="mb-4 text-center">Listado de Tickets</h4>
             <asp:Button ID="btnActualizar" runat="server" CssClass="btn btn-info text-white" Text="Actualizar" OnClick="btnActualizar_Click" />
            <div class="mb-3 text-end">
                <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control d-inline-block" Width="250px" placeholder="Buscar..."></asp:TextBox>
                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-primary ms-2" OnClick="btnBuscar_Click" />
            </div>
            <div class="scrollable-table-container">
             <asp:GridView ID="gvTickets" runat="server" AutoGenerateColumns="False"
              CssClass="table-custom"
              DataKeyNames="Id"
              OnRowCommand="gvTickets_RowCommand"
              RowStyle-BackColor="White"
              AlternatingRowStyle-BackColor="#f9f9f9"
              HeaderStyle-BackColor="#e5e7eb" HeaderStyle-ForeColor="Black">
                <Columns>
                    <asp:TemplateField HeaderText="Editar">
                        <ItemTemplate>
                            <asp:Button ID="btnEditar" runat="server" Text="Editar" CssClass="btn btn-primary"
                                CommandArgument='<%# Eval("Id") %>' CommandName="EditarTicket" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Id" HeaderText="ID" />
                    <asp:TemplateField HeaderText="Nombre">
                    <ItemTemplate>
                       <%# Eval("Nombre") + " " + Eval("Apellido") %>
                    </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Departamento" HeaderText="Departamento" />
                    <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                    <asp:BoundField DataField="Correo" HeaderText="Correo" />
                    <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                    <asp:TemplateField HeaderText="Descripción">
                        <ItemTemplate>
                            <div class="descripcion-columna" title='<%# Eval("Descripcion") %>'>
                                <%# Eval("Descripcion") %>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                     <asp:BoundField DataField="AsignadoA" HeaderText="Asignado" />
                    <asp:TemplateField HeaderText="Estatus">
                 <ItemTemplate>
                   <asp:Label ID="lblEstatus" runat="server" Text='<%# Eval("Estatus") %>'
                       CssClass='<%# Eval("Estatus").ToString() == "Recibido" ? "estatus-recibido" : (Eval("Estatus").ToString() == "En Proceso" ? "estatus-enproceso" : "estatus-cerrado") %>'>
                   </asp:Label>
                 </ItemTemplate>
                 </asp:TemplateField>
                    <asp:BoundField DataField="FechaRegistro" HeaderText="Fecha Registro" 
                     DataFormatString="{0:dd MMM yyyy HH:mm}" HtmlEncode="False" />
                    <asp:TemplateField HeaderText="Fecha Fin">
                      <ItemTemplate>
                         <%# Eval("FechaFin") != DBNull.Value ? 
                          string.Format("{0:dd MMM yyyy HH:mm}", Eval("FechaFin")) : "" %>
                      </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

        <!-- Modal Bootstrap 5 -->
        <div class="modal fade" id="modalAsignar" tabindex="-1" aria-labelledby="modalAsignarLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content p-4">
                    <div class="modal-header">
                        <h5 class="modal-title">Asignar Ticket</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <div class="form-group mb-2">
                          <label><strong>Nombre:</strong></label>
                           <asp:Label ID="lblNombre" runat="server" CssClass="form-control-plaintext text-dark" />
                       </div>
                              <div class="form-group mb-2">
                                 <label><strong>Apellido:</strong></label>
                             <asp:Label ID="lblApellido" runat="server" CssClass="form-control-plaintext text-dark" />
                        </div>
                          <div class="form-group mb-2">
                            <label><strong>Departamento:</strong></label>
                             <asp:Label ID="lblDepartamento" runat="server" CssClass="form-control-plaintext text-dark" />
                           </div>
                             <div class="form-group mb-3">
                                 <label><strong>Descripción:</strong></label>
                                   <div id="descripcionContainer" runat="server" class="descripcion-scroll"></div>
                               </div>


                            <label>Asignado A:</label>
                            <asp:DropDownList ID="ddlAsignadoA" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                        <div class="mb-3">
                            <label>Estatus:</label>
                            <asp:DropDownList ID="ddlEstatus" runat="server" CssClass="form-select">
                                <asp:ListItem Value="Asignado">Asignado</asp:ListItem>
                                <asp:ListItem Value="En Proceso">En Proceso</asp:ListItem>
                                <asp:ListItem Value="Cerrado">Cerrado</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnGuardarAsignacion" runat="server" CssClass="btn btn-success" Text="Guardar" OnClick="btnGuardarAsignacion_Click" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
