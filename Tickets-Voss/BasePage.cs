using System;
using System.Web.UI;
using Tickets_Voss.Models;
using static Tickets_Voss.Models.NotificationHelper; // ← Namespace correcto

namespace Tickets_Voss
{
    public class BasePage : Page
    {
        public void Alert(string message, NotificationType notificationType, string position = "top-end", int timer = 1500, bool showConfirmButton = false)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("El mensaje no puede estar vacío.", nameof(message));

            string icon = notificationType.ToString().ToLower(); // Asegura que el icono sea válido para SweetAlert
            string script = $@"
                Swal.fire({{
                    position: '{position}',
                    icon: '{icon}',
                    title: '{message}',
                    showConfirmButton: {showConfirmButton.ToString().ToLower()},
                    timer: {timer}
                }});
            ";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "SweetAlert", script, true);
        }
    }
}