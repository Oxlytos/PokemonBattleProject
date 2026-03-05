using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Models
{
    public class TargetModel
    {
        enum Target
        {
            None, //
            Self, //Swordsdance
            SingularOpponent, //Flamethrower
            Ally, //Heal pulse
            BothSelfAlly, //reflect
            BothOpponents, //Muddy Water
            AllOpponents, //Muddy water i triple battle?
            Everyone, //Explosion

        }
    }
}
