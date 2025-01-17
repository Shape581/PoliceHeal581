using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Life;
using Life.UI;
using Life.Network;
using ModKit;
using Utils;
using Format = ModKit.Helper.TextFormattingHelper;
using SQLite;
using ModKit.Helper;
using Life.BizSystem;

namespace PoliceHeal581
{
    public class Main : ModKit.ModKit
    {
        public Main(IGameAPI api) : base(api) 
        {
            PluginInformations = new ModKit.Interfaces.PluginInformations(AssemblyHelper.GetName(), "1.0.0", "Shape581");
        }

        public override void OnPluginInit()
        {
            base.OnPluginInit();
            AAMenu.Menu.AddBizTabLine(PluginInformations, new List<Life.BizSystem.Activity.Type> { Activity.Type.LawEnforcement }, null, Format.Color("Premier Secours", Format.Colors.Info), aaMenu =>
            {
                var player = PanelHelper.ReturnPlayerFromPanel(aaMenu);
                var closestPlayer = player.GetClosestPlayer(false);
                if (closestPlayer == null)
                {
                    player.Notify(Format.Color("Erreur", Format.Colors.Error), "Il n'y a personne a proximité.", NotificationManager.Type.Error);
                    return;
                }
                if (closestPlayer.setup.Networkhealth > 0)
                {
                    player.Notify(Format.Color("Erreur", Format.Colors.Error), "L'individu n'est pas inconscient.", NotificationManager.Type.Error);
                    return;
                }
                Timer.Run(player, async () =>
                {
                    player.setup.NetworkisFreezed = true;
                    closestPlayer.setup.TargetShowCenterText(Format.Color("Information", Format.Colors.Info), "Un Policier vous fait les Premier Secours...", 5f);
                    player.setup.TargetShowCenterText(Format.Color("Information", Format.Colors.Info), "Vous faites les Premier Secours a l'invidivu.", 5f);
                    await Task.Delay(5000);
                    var random = new Random();
                    var number = random.Next(1, 5);
                    if (number == 3)
                    {
                        player.Notify(Format.Color("Avertissement", Format.Colors.Warning), "Vous avez échouer les Premier Secours.", NotificationManager.Type.Warning);
                        player.setup.TargetPlayClaironById(0.1f, 2);
                        closestPlayer.Notify(Format.Color("Information", Format.Colors.Info), "Le Policier a échouer les Premier Secours.");
                        return;
                    }
                    closestPlayer.Notify(Format.Color("Succès", Format.Colors.Success), "Le Policier a réuissi les Premier Secours.", NotificationManager.Type.Success);
                    player.Notify(Format.Color("Succès", Format.Colors.Success), "Vous avez réussi les Premier Secours.", NotificationManager.Type.Success);
                    closestPlayer.setup.Networkhealth = 15;
                    player.setup.NetworkisFreezed = false;
                });
            });
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[PoliceHeal581] - Intialisé !");
            Console.ResetColor();
        }
    }

    public class Timer : Cooldown.Invididual.Component<Timer> { }
}
