using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

public class GameDialog
{
    public string speaker {  get; set; }    
    public string emotion {  get; set; }    
    public bool isLeft {  get; set; }   
    public string text {  get; set; }
    public string branch_id {  get; set; }
    public string action_name { get; set; }
    public List<MultipleChoiceModel> multiple_choice {  get; set; }   = new List<MultipleChoiceModel>(); 
    public string ToJson()
    {
     return   JsonSerializer.Serialize(this);
    }

    //public GameDialog Clone()
    //{
    //    return new GameDialog(this);
    //}
}
   
public class MultipleChoiceModel
{
    public string choice_head { get; set; } 
    public string branch_name {  get; set; }    
}

//public class DialogueMetaData
//{
//    public string choice_text { get; set; }
//    public string choice_branch { get; set; }
//}

public class Jsonn
{
    public List<GameDialog> dialogs { get; set; }
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
}
namespace DialogMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        List<string> list_speakers = new List<string>();
        List<string> list_emotion = new List<string>();
        int thisLine=1;
        bool isLeft;
        string main_brach= "Main_Branch";
        string last_branch_combo;
        private const string FILE_PATH = "Dialogues";
        //object last_b_combo ;
        List<MultipleChoiceModel> choices_per_dialogue=new List<MultipleChoiceModel>();
        List<MultipleChoiceModel> inlistbox = new List<MultipleChoiceModel>();
        List<List<MultipleChoiceModel>> choices_all=new List<List<MultipleChoiceModel>>();
        List<GameDialog> dialogs=new List<GameDialog>();
        public MainWindow()
        {
          
            InitializeComponent();
            combo_branch.Items.Add(main_brach);
            last_branch_combo =(string) combo_branch.Items[0];
            combo_branch.SelectedValue= last_branch_combo;
            //last_b_combo =  combo_branch.SelectedValue;
            read_parameter_file();
        }

        private void read_parameter_file()
        {
            string sp_filepath = "parameters\\speakers.txt";
            string emo_filepath = "parameters\\emotions.txt";
            if (File.Exists(sp_filepath))
            {
                var list = File.ReadAllLines(sp_filepath);
                foreach(var speaker in list) 
                    {
                    //list_speakers.Add(speaker);
                    combo_speakers.Items.Add(speaker);  
                    }
            }
            if (File.Exists(emo_filepath))
            {
                var list = File.ReadAllLines(emo_filepath);
                foreach (var emo in list)
                {
                    //list_speakers.Add(speaker);
                    combo_emo.Items.Add(emo);
                }
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
           n_chars.Text= "Number of Characters " + text.Text.Length.ToString() +"//150";
            if (e.Key==Key.Enter)
            {
                if (text.Text.Length <= 150)
                {


                    if (Submit_Dialog())
                    {
                        if(thisLine-1 ==dialogs.Count)
                        {
                            text.Text = "";
                        }
                        else
                        {
                            thisLine++;
                            load_line(thisLine);
                        }
                     
                    }
                }
                else
                {
                    MessageBox.Show("the number of character is exceed!","Too much character");
                }
            }
        }


        private bool Submit_Dialog()
        {

            if (combo_speakers.SelectedItem != null && combo_emo.SelectedItem != null
                && !text.Text.Equals("") && combo_branch.SelectedItem!=null)
            {

                var dialog = new GameDialog();
                dialog.text = text.Text;
                dialog.isLeft = isLeft;
                dialog.speaker = combo_speakers.SelectedItem.ToString();
                dialog.emotion = combo_emo.SelectedItem.ToString();
                dialog.branch_id = (string)combo_branch.SelectedValue;
                dialog.action_name = action_textbox.Text;

                last_branch_combo = dialog.branch_id;
           


                       Debug.WriteLine("Line Saved");
                text.Text = "";
                action_textbox.Text = "";




                        Debug.WriteLine("here ***********");
   
                  
                        if (inlistbox.Count > 0)
                {
                    choices_per_dialogue.AddRange(inlistbox);
  
                }
        
                      
                        
                        dialog.multiple_choice = new List<MultipleChoiceModel>(choices_per_dialogue); ;
                      

                        add_to_list(dialog, thisLine);
                        choices_per_dialogue = new List<MultipleChoiceModel>();
                        
                        //Clear UI
                        ListBox_Chooices.Items.Clear();
                        CheckForBranchs();
                        //dialog = choice;
                        return true;
       
                
            }
            else { return false; }

            

        }
        private void add_to_list(GameDialog dialog,int line)

        {
            if (dialogs.Count == line - 1)
            {
                thisLine = thisLine + 1;
                n_lines.Text = "line : " + (thisLine).ToString();

                dialogs.Add(dialog);
            }
            else
            {
                n_lines.Text = "line : " + (dialogs.Count + 1).ToString();
                dialogs.RemoveAt(thisLine - 1);
                dialogs.Insert(thisLine-1, dialog);
            }

        }
        private bool save()
        {
            try
            {
                var path = FILE_PATH +"\\"+ name_file_textbox.Text + ".json";
                FileStream file;
                //File.
                if (!File.Exists(path))
                {
                    Directory.CreateDirectory(FILE_PATH);
                    file = File.Create(path);
                }
                else
                {
                   
                    file = File.Open(path, FileMode.Open);
                }
                file.Close();
                string json;
                Jsonn j = new Jsonn();
                j.dialogs = dialogs;
                json = j.ToJson();
                File.WriteAllText(path, json, Encoding.UTF8);
                return true;    
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "Not able to save file");
                return false;
            
            }
   
        }
        
        private void left_radio_Checked(object sender, RoutedEventArgs e)
        {
            isLeft = true;
            Debug.WriteLine(isLeft); 
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            isLeft =false;
            Debug.WriteLine(isLeft);
        }

        private void prev_btn_Click(object sender, RoutedEventArgs e)
        {
            if (thisLine != 1)
            {


                load_line(thisLine - 1);
                thisLine--;
                CheckForBranchs();
            }
            else
            {
                //MessageBox.Show("there is no previous line");
            }
        }
        private void load_line(int line)
        {
            int iterator=1;
            foreach(var item in dialogs)
            {
                if(iterator==line)
                {
                    Set_UI(item, iterator);
                    break;
                }
                iterator++;
            }
        }

        private void Set_UI(GameDialog dialog,int line_)
        {
           
            combo_emo.SelectedItem = dialog.emotion;
            action_textbox.Text = dialog.action_name;
            combo_speakers.SelectedItem = dialog.speaker;
            left_radio.IsChecked = dialog.isLeft;
            text.Text = dialog.text;

                n_lines.Text = "Line : " + (line_).ToString();
            if(dialog.text!=null)
            {
                n_chars.Text = "Number of Characters " + dialog.text.Length.ToString() + "//150";
            }
            else
            {
                n_chars.Text = "Number of Characters " + 0.ToString() + "//150";
            }

            if (dialog.multiple_choice.Count == 0)
            { ListBox_Chooices.Items.Clear(); }
            else
            {
                inlistbox.Clear();
                foreach (var ch in dialog.multiple_choice)
                {
                    add_to_listbox_choice(ch);
                    inlistbox.Add(ch);
                }
            }
            last_branch_combo = dialog.branch_id;
            CheckForBranchs();
        }
        private void next_line_btn_Click(object sender, RoutedEventArgs e)
        {
            if(thisLine-1 != dialogs.Count) 
                {
                
            load_line(thisLine+1);
            thisLine++;
                CheckForBranchs();
            }
            else
            {
                thisLine=dialogs.Count + 1;

                Set_UI(new GameDialog(), thisLine);
            }
        }

        private void save_line_btn_Click(object sender, RoutedEventArgs e)
        {
            Submit_Dialog();
            thisLine++;
            load_line(thisLine);

        }

        private void delete_btn_Click(object sender, RoutedEventArgs e)
        {
            if (thisLine-1 < dialogs.Count)
            {
                if (dialogs.Count > 0)
                {
                    if (thisLine - 1 > dialogs.Count)
                    {
                        thisLine = dialogs.Count;
                    }
                    Debug.WriteLine(thisLine);
                    Debug.WriteLine(dialogs.Count);
                    dialogs.RemoveAt(thisLine - 1);
                    Set_UI(new GameDialog(), thisLine);
                    load_line(thisLine);
                }
            }
        }



        private void save_file_btn_Click(object sender, RoutedEventArgs e)
        {
            if(save())
            {
                MessageBox.Show("Successfully saved");
            }
            else
            {
             
            }
        }

        private void load_file_btn_Click(object sender, RoutedEventArgs e)
        {

            string fileNameToOpen=FILE_PATH+ "\\" + name_file_textbox.Text+".json";
            if (File.Exists(fileNameToOpen))
            {
                string json=File.ReadAllText(fileNameToOpen);   
                dialogs.Clear();
                var obj=JsonSerializer.Deserialize(json, typeof(Jsonn));
                var ss = (Jsonn) obj;
                dialogs = ss.dialogs;
                load_line(thisLine);
            }
            else
            {
                Directory.CreateDirectory(FILE_PATH);
               
            }
                
        }


        private void ChoiceKeyDown_branch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                if (choice_text.Text != "")
                {
                    if(choice_branch.Text=="")
                    {

                        choice_branch.Text = Make_Name(thisLine, choices_per_dialogue.Count + 1);
                    }
                    if (!SaveChoice(choice_text.Text, choice_branch.Text))
                    {
                        MessageBox.Show("Branch name is unique ");
                    }
                    else { 
                    }
                }
            }
        }

        private void ChoiceKeyDown_text(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter) { 
            
                if(choice_text.Text !="" )
                {
                    if (choice_branch.Text != "")
                    {
                        // save
                    }
                    else
                    {
                        // It has be unique 
                        choice_branch.Text = Make_Name(thisLine, choices_per_dialogue.Count + 1);
                        //choice_branch.Text = thisLine.ToString() + "_" + "B" + (choices_per_dialogue.Count + 1).ToString ()+ ":";
                    }
                  if(  !SaveChoice(choice_text.Text, choice_branch.Text))
                    {
                        MessageBox.Show("Branch name is unique ");
                    }
                }
                else
                {
                 
                }
            
            }

        }

        private string Make_Name(int lineNumber,int i)
        {
            string ln = lineNumber.ToString();
            string i_n = i.ToString();
            string b_name = "branch_" + ln + "_"+ i_n;


            return b_name;
        }
        private bool SaveChoice(string text,string branch)
        {
            //ShowListTome();
            foreach (var ch in choices_all)
            {
                foreach (var item in ch)
                {
                    if (item.branch_name .Equals(branch))
                    {
                        return false;
                    }
                }
            }
            
            MultipleChoiceModel choice=new MultipleChoiceModel();
            choice.choice_head = text;
            choice.branch_name = branch;

            choices_per_dialogue.Add(choice);

            add_to_listbox_choice(choice);
            choices_all.Add(choices_per_dialogue);
            choice_branch.Text = "";
            choice_text.Text = "";
            return true;
        }
        void CheckForBranchs()
        {
            combo_branch.SelectedItem = null;
            combo_branch.Items.Clear();
            List<string> branch_lists = new List<string>();
            List<GameDialog> prev_dialog = new List<GameDialog>();
        



            for (int i = 0; i < thisLine - 1; i++)
            {
                prev_dialog.Add(dialogs[i]);
             
                if (dialogs[i].multiple_choice.Count != 0)
                {

                    foreach (var ch in dialogs[i].multiple_choice)
                    {
                        branch_lists.Add(ch.branch_name);
                    }

                }

            }

            branch_lists = del_branch_starter(branch_lists, prev_dialog);

            foreach (var ch in branch_lists)
            {
                combo_branch.Items.Add(ch);
            }

            bool is_in = false;
            if (last_branch_combo!=null)
            if (last_branch_combo.Equals(main_brach)&& branch_lists.Count==0)
            {
                combo_branch.Items.Add(last_branch_combo);
                combo_branch.SelectedItem = main_brach;

            }
            else
            {
 
                    foreach (var ch in branch_lists)
                    {
                        Debug.WriteLine(ch);

                        if (last_branch_combo.Equals(ch))
                        {
                            is_in = true;
                        break;
                        }

                    }
              
                    if (is_in)
                    {
                        combo_branch.SelectedValue = last_branch_combo;
                    }
                    else
                    {
                        combo_branch.SelectedValue = null;

                    }
            }
        }




        private List<string> del_branch_starter(List<string> branchs,List<GameDialog> prevD)
        {
            List<string> branch_starter=new List<string>();
            foreach (var ch in prevD)
            {
                if(ch.multiple_choice.Count!=0)
                {
                    branch_starter.Add(ch.branch_id);
                }
            }

            foreach(var ch in branch_starter)
            {
                branchs.Remove(ch);
            }
            return branchs;
        }


        void add_to_listbox_choice(MultipleChoiceModel choice)
        {
            ListBox_Chooices.Items.Add(choice.choice_head + "  |  " + choice.branch_name);
        }


       
    }
}