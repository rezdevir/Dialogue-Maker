using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DialogMaker.Model
{
    public class GameDialogue
    {
        public string speaker { get; set; }
        public string emotion { get; set; }
        public bool isLeft { get; set; }
        public string text { get; set; }
        public string branch_id { get; set; }
        public string action_name { get; set; }
        public List<DialogueStyleModel> styles { get; set; } = new List<DialogueStyleModel>();
        public List<MultipleChoiceModel> multiple_choice { get; set; } = new List<MultipleChoiceModel>();
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class DialogueStyleModel
    {

        public DStyle style { get; set; }

    }


    public class DStyle
    {
        public string style_type { get; set; }
        public int start { get; set; }
        public int end { get; set; }
        public string data { get; set; }
    }


    public class MultipleChoiceModel
    {
        public string choice_head { get; set; }
        public string branch_name { get; set; }
    }


    public class DialogueToJson
    {
        public List<GameDialogue> dialogs { get; set; }
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }

}
