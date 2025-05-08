<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registro.aspx.cs" Inherits="Tickets_Voss.Registro" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registrar Usuario - IT Voss</title>

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
            max-width: 120px;
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
            background-color: #db2777;
        }

        .link-volver {
            display: block;
            margin-top: 1rem;
            text-align: center;
            color: #6b7280;
            text-decoration: none;
        }

        .link-volver:hover {
            color: #ec4899;
            text-decoration: underline;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container py-5">
            <div class="text-center mb-4">
                <img src="Content/images/Voss-logo.svg" alt="Logo Voss" class="logo" />
                <h4 class="mt-2">Registrar Nuevo Usuario</h4>
            </div>
            <div class="row justify-content-center">
                <div class="col-md-6">
                    <div class="form-card">
                        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control custom-input mb-3" placeholder="Nombre"></asp:TextBox>
                        <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control custom-input mb-3" placeholder="Correo"></asp:TextBox>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control custom-input mb-3" TextMode="Password" placeholder="Contraseña"></asp:TextBox>
                        <asp:Button ID="btnRegistrar" runat="server" Text="Registrar" CssClass="btn btn-rosa w-100" OnClick="btnRegistrar_Click" />
                        <a href="IT.aspx" class="link-volver">Login IT</a>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
