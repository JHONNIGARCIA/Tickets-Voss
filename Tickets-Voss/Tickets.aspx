<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tickets.aspx.cs" Inherits="Tickets_Voss.Tickets" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Solicitud de Servicio IT-Voss-Laguna</title>

    <!-- MDB -->
    <link href="https://cdnjs.cloudflare.com/ajax/libs/mdb-ui-kit/6.4.1/mdb.min.css" rel="stylesheet" />
       <link href="Content/sweetalert2.css" rel="stylesheet" />
       <script src="Scripts/jquery-3.7.0.min.js"></script>
       <script src="Scripts/sweetalert2.js"></script>

    <style>
        body {
            background: #f3f4f6;
            font-family: 'Arial', sans-serif;
        }

        .logo {
            max-width: 150px;
        }

        .form-card {
            background-color: white;
            border-radius: 20px;
            padding: 2rem;
            box-shadow: 0 5px 25px rgba(0, 0, 0, 0.08);
        }

        .custom-input {
            padding: 1rem;
            font-size: 1rem;
            border-radius: 12px;
            border: 1px solid #d1d5db;
        }

        .custom-input:focus {
            border-color: #ec4899 !important;
            outline: none;
            box-shadow: none !important;
        }

        .btn-rosa {
            background-color: #ec4899;
            color: white;
            font-weight: 500;
            border-radius: 10px;
            padding: 0.75rem 1rem;
            border: none;
        }

        .btn-rosa:hover {
            background-color: #ec4899;
        }

        .image-side {
            width: 100%;
            object-fit: cover;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server" ID="ScriptManager1" />
        <div class="container py-5">
            <div class="text-center mb-4">
                <img src="Content/images/Voss-logo.svg" alt="Logo Voss" class="logo" />
                <h4 class="mt-2">Solicitud de Servicio IT-Voss-Laguna</h4>
            </div>
            <div class="row justify-content-center align-items-center">
                <div class="col-md-6 mb-4 text-center">
                    <img src="Content/images/imagen.png" class="img-fluid rounded shadow-sm image-side" alt="Imagen decorativa" />
                </div>
                <div class="col-md-6">
                    <div class="form-card">
                        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control custom-input mb-3" placeholder="Nombre"></asp:TextBox>
                        <asp:TextBox ID="txtApellido" runat="server" CssClass="form-control custom-input mb-3" placeholder="Apellido"></asp:TextBox>
                        <asp:DropDownList ID="ddlDepartamento" runat="server" CssClass="form-select custom-input mb-3">
                            <asp:ListItem Selected="True" Disabled="True">Selecciona tu departamento</asp:ListItem>
                            <asp:ListItem>Calidad</asp:ListItem><asp:ListItem>Compras</asp:ListItem><asp:ListItem>Direccion</asp:ListItem><asp:ListItem>Extrusion</asp:ListItem>
                            <asp:ListItem>Finanzas</asp:ListItem><asp:ListItem>Ingenieria</asp:ListItem><asp:ListItem>IT</asp:ListItem><asp:ListItem>Logistica</asp:ListItem>
                            <asp:ListItem>Mantenimiento</asp:ListItem><asp:ListItem>Logistica Mejora Continua</asp:ListItem><asp:ListItem>Recursos Humanos</asp:ListItem>
                            <asp:ListItem>Seguridad</asp:ListItem><asp:ListItem>Sistema de calidad</asp:ListItem><asp:ListItem>Produccion</asp:ListItem>
                        </asp:DropDownList>

                        <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control custom-input mb-3" placeholder="Telefono"></asp:TextBox>
                        <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control custom-input mb-3" placeholder="Correo"></asp:TextBox>

                        <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-select custom-input mb-3">
                            <asp:ListItem Selected="True" Disabled="True">Selecciona una categoria</asp:ListItem>
                            <asp:ListItem>Aplicaciones</asp:ListItem><asp:ListItem>Base de datos</asp:ListItem><asp:ListItem>Computadora</asp:ListItem><asp:ListItem>Correo</asp:ListItem>
                            <asp:ListItem>Cuentas de usuario</asp:ListItem><asp:ListItem>Red</asp:ListItem><asp:ListItem>Telefono</asp:ListItem><asp:ListItem>Otro</asp:ListItem>
                        </asp:DropDownList>

                        <asp:TextBox ID="txtDescripcion" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control custom-input mb-3" placeholder="Descripcion"></asp:TextBox>

                        <asp:FileUpload ID="fileUpload" runat="server" CssClass="form-control mb-3" />

                        <asp:Button ID="btnEnviar" runat="server" CssClass="btn btn-rosa w-100" Text="ENVIAR SOLICITUD" OnClick="btnEnviar_Click" />
                    </div>
                </div>
            </div>
        </div>
    </form>

<script>
    document.getElementById('<%= btnEnviar.ClientID %>').addEventListener('click', function (e) {
        const nombre = document.getElementById('<%= txtNombre.ClientID %>').value.trim();
        const correo = document.getElementById('<%= txtCorreo.ClientID %>').value.trim();

        if (!nombre) {
            Swal.fire({
                title: 'Error',
                text: 'El campo Nombre es obligatorio.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
            e.preventDefault();
            return;
        }

        if (!correo.endsWith('@voss.net')) {
            Swal.fire({
                title: 'Error',
                text: 'El correo debe terminar en @voss.net.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
            e.preventDefault();
            return;
        }
    });
</script>


</body>
</html>
