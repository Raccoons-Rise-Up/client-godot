using Godot;
using KRU.Networking;

namespace KRU.UI
{
    public class UIStore : Control
    {
        private void _on_Btn_Hut_pressed()
        {
            ENetClient.PurchaseItem(0);
        }

        private void _on_Btn_Farm_pressed()
        {
            ENetClient.PurchaseItem(1);
        }
    }

}
