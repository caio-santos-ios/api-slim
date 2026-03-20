using api_slim.src.Interfaces;
using api_slim.src.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebPush;
using System.Text.Json;

namespace api_slim.src.Controllers
{
    [Route("api/smclick")]
    [ApiController]
    public class SmClickController(ISmClickService service) : ControllerBase
    {
        // [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var subscription = new PushSubscription(
                "https://fcm.googleapis.com/fcm/send/fM_b5npZf_M:APA91bGszg6oJyFm7T8sW-EXPnfvU_lkayrgYNePGcM0PvlxYJYuF2PA1grmX4G3NJabxIdy5gWGtwsENiKs13ocAQswPnsWZXeszK7-6OMDvJmP3BInh9Zy-vAYW9VEcRn4WRoVz04Q",
                "BDk95ab5mG1ZRHbqlOC1BbMx13uO6BxesmipGHxIGbFXGqc3nK1r3PuHmEISEfj9DT35481kDFWi2DVtrPwhAGQ",
                "U0yCRi3TeXulWZpjrIkR9A"
            );

            // 2. Suas Chaves VAPID
            var vapidDetails = new VapidDetails(
                "mailto:caiodev.fullstack@gmail.com", 
                "BKAi4Ae35cMd0JtCRVgIuHq6tjlqaN0Va0AifE1OzuldnKWkoGILA1F5qRr6iYOh6rcKr_3cp14qEFeNmp6olhs", 
                "FOv1UoSGCo69pQUIak5zpi_PtOKQZ7TKTvUGNnN0wN8"
            );

            // 3. Payload (O JSON que o seu sw.js espera ler)
            var payload = JsonSerializer.Serialize(new { 
                title = "Pasbem Informa", 
                body = "Sua saúde foi monitorada com sucesso! ✅" 
            });

            var webPushClient = new WebPushClient();
            try
            {
                webPushClient.SendNotification(subscription, payload, vapidDetails);
                Console.WriteLine("Notificação enviada!");
            }
            catch (WebPushException ex)
            {
                Console.WriteLine("Erro ao enviar: " + ex.StatusCode);
            }
            
            return StatusCode(200, new { });
        }
    }
}