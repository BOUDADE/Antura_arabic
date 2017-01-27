using EA4S.Core;
using EA4S.LivingLetters;
using EA4S.MinigamesAPI;
using UnityEngine;

namespace EA4S.Assessment
{
    public class SortingTicket : MonoBehaviour
    {
        public int number = -1;
        public ILivingLetterData data = null;
    }

    public static class AddTicketGoExtension
    {
        public static SortingTicket AddTicket( this Answer answ, int ticketN)
        {
            var comp = answ.gameObject.AddComponent< SortingTicket>();
            comp.data = answ.gameObject.GetComponent< LetterObjectView>().Data;
            comp.number = ticketN;
            return comp;
        }

        public static int GetTicket( this Answer answ)
        {
            return answ.gameObject.GetComponent<SortingTicket>().number;
        }
    }
}
