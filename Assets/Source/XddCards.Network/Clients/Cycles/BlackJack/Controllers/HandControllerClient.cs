using System;
using System.Collections.Generic;
using System.Linq;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Cycles.BlackJack.Controllers;
using XddCards.Grpc.Cycles.BlackJack.Controllers;

namespace Xdd.Network.Clients.Cycles.BlackJack.Controllers
{
    internal class HandControllerClient : ControllerClient<HandControllerGrpc.HandControllerGrpcClient>, IHandController
    {
        public IHand[] Hands => _Hands.ToArray();
        public List<HandClient> _Hands = new();

        private List<UserClient> users;

        public HandControllerClient(List<UserClient> users)
        {
            this.users = users;

            CreateListener(
                   client => client.OnHandsChange(new HandsRequest()),
                   async response =>
                   {
                       foreach (var hand in response.Hands)
                       {
                           var handClient = _Hands.FirstOrDefault(x => x.Id == hand.Id);

                           if (handClient == null)
                           {
                               handClient = new HandClient(hand.Id);
                               _Hands.Add(handClient);
                           }

                           if (hand.Owner != null)
                           {
                               handClient.owner = this.users.FirstOrDefault(x => x.Id == hand.Owner.Id);
                               if (handClient.owner == null)
                               {
                                   var user = await UserClient.New(hand.Owner.Id, hand.Owner.Nickname);
                                   users.Add(user);
                                   handClient.owner = user;
                               }
                           }
                       }
                   });
        }
    }
}
