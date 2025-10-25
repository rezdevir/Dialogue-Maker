using DialogMaker.Model;
using DialogMaker.ViewModel;
using Microsoft.Win32;
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





/// First Released Version for dialogue maker 
namespace DialogMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        //List<string> list_speakers = new List<string>();
        //List<string> list_emotion = new List<string>();
        string text_animatation="normal";
        List<DialogueStyleModel> dialogueStyles = new List<DialogueStyleModel>();
        int thisLine=1;
        bool isLeft;
        static string main_brach = "Main_Branch";
        string last_branch_combo;
        private const string FILE_PATH = "Dialogues";
        private const string FILE_PATH_PARA = "parameters";
       
        private string MainCharacter="";
        //object last_b_combo ;
     
        List<MultipleChoiceModel> dialogueChoiuces = new List<MultipleChoiceModel>();
        List<List<MultipleChoiceModel>> choices_all=new List<List<MultipleChoiceModel>>();
        List<GameDialogue> dialogs=new List<GameDialogue>();

        public MainWindow()
        {
          
            InitializeComponent();
            combo_branch.Items.Add(main_brach);
            last_branch_combo =(string) combo_branch.Items[0];
            combo_branch.SelectedValue= last_branch_combo;
            //last_b_combo =  combo_branch.SelectedValue;
            if(!Directory.Exists(FILE_PATH))
                Directory.CreateDirectory(FILE_PATH);
            read_parameter_file();

            Closed += closedB;


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
                    
                    combo_speakers.Items.Add(SpeakersHandler(speaker));  
                    }
            }
            else
            {
                Directory.CreateDirectory(sp_filepath.Split("\\")[0]);
                File.Create(sp_filepath);
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
            else {
                Directory.CreateDirectory(emo_filepath.Split("\\")[0]);
                File.Create(emo_filepath);
            }

        }
        private string SpeakersHandler(string speaker)
        {
            if (speaker.Contains("###"))
            {
                Debug.WriteLine("HEre ");

                speaker = speaker.Replace("###", "").Trim();
                MainCharacter = speaker;
            }
               
            
            return speaker; 
        }


        private void Combo_Value_Changed_Manager(object sender, EventArgs e)

        {
            if (combo_speakers.SelectedItem == null) return;
            if (((string)combo_speakers.SelectedItem).Equals(MainCharacter))
            {
                left_radio.IsChecked = true;
            }
            else
            {
                right_radio.IsChecked = true;
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
                            prev_btn.IsEnabled = true;
                            load_line(thisLine);
                        }
                     
                    }
                }
                else
                {
                    MessageBox.Show("The number of character is exceed!","Too much character");
                }
            }
        }


        private bool Submit_Dialog()
        {

            if (combo_speakers.SelectedItem != null && combo_emo.SelectedItem != null
                && !text.Text.Equals("") && combo_branch.SelectedItem!=null)
            {
                
                var dialog = new GameDialogue();
                dialog.text = text.Text;
                dialog.isLeft = isLeft;
                dialog.speaker = combo_speakers.SelectedItem.ToString();
                dialog.emotion = combo_emo.SelectedItem.ToString();
                dialog.branch_id = (string)combo_branch.SelectedValue;
                dialog.action_name = action_textbox.Text;

                //last_branch_combo = dialog.branch_id;
           


                Debug.WriteLine("Line Saved");
                text.Text = "";
                action_textbox.Text = "";
                choice_text.Text = "";
                choice_branch.Text = "";

                        dialog.styles = new List<DialogueStyleModel>(dialogueStyles);

                if(dialogs.Count> thisLine - 1)
         
                if (dialogs[thisLine - 1].multiple_choice.Count > 0)
                {
                    dialog.multiple_choice = new List<MultipleChoiceModel>(dialogs[thisLine - 1].multiple_choice);
                        
                  

                }

                foreach (var dd in dialogueChoiuces)
                    dialog.multiple_choice.Add(dd);


                //dialog.multiple_choice = new List<MultipleChoiceModel>(dialogueChoiuces);
                        add_to_list(dialog, thisLine);
                        dialogueChoiuces.Clear();
                        dialogueChoiuces = new List<MultipleChoiceModel>();
              
            
                        
                        
                        dialogueStyles = new List<DialogueStyleModel>();

                //Clear UI
                //ListBox_Chooices.Items.Clear();

                #region checkforbranch

                //CheckForBranchs();

               
                BranchManager(thisLine);
                if (dialog.multiple_choice.Count == 0)
                    combo_branch.SelectedItem = dialog.branch_id;
                DeleteListViewBranch(); 
                #endregion checkforbranch
                //dialog = choice;
                return true;
       
                
            }
            else { return false; }

            

        }
        private void add_to_list(GameDialogue dialog,int line)

        {
            if (dialogs.Count == line - 1)
            {
                thisLine = thisLine + 1;
                n_lines.Text = "Line : " + (thisLine).ToString();
                Debug.WriteLine("line is:" + line + " and eq to dialogs.Count");
                dialogs.Add(dialog);
            }
            else
            {
                n_lines.Text = "Line : " + (dialogs.Count + 1).ToString();
                dialogs.RemoveAt(thisLine - 1);
                dialogs.Insert(thisLine-1, dialog);
                Debug.WriteLine("line is:" + line +"and Dialogue is: "+dialogs.Count);
            }

        }


        private bool CheckBranchConsistency()
        {

            foreach(var d in dialogs)
            {
                bool flag = false;
                if (d.branch_id.Equals(main_brach)) continue;
                foreach(var dd in dialogs)
                {
                    foreach(var m in dd.multiple_choice)
                    {
                        if (d.branch_id.Equals(m.branch_name))
                        {
                            flag=true; break;
                        }
                    }
                  
                }
                if(!flag ) {
                    MessageBox.Show("Branch \""+ d.branch_id+ "\" Not Found");

                    return false; } 
            }
            return true;
        }
        private bool save()
        {
            if(!CheckBranchConsistency()) {  return false; }


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
                DialogueToJson j = new DialogueToJson();
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






        #region Radiobtn
        private void left_radio_Checked(object sender, RoutedEventArgs e)
        {
            isLeft = true;
           
        }

        private void right_radio_Checked(object sender, RoutedEventArgs e)
        {
            isLeft =false;
            
        }
        
        private void normal_radio(object sender, RoutedEventArgs e)
        {
            text_animatation = "normal";

        }

        private void waving_radio(object sender, RoutedEventArgs e)
        {
            text_animatation = "waving";

        }
        private void shaking_radio(object sender, RoutedEventArgs e)
        {
            text_animatation = "shaking";
        }
        private void big_radio(object sender, RoutedEventArgs e)
        {
            text_animatation = "bigger";
        }
        #endregion Radiobtn





        string the_size_w;
        private void check_size_text_box(object sender, RoutedEventArgs e)
        {
          
            //Debug.WriteLine(TextBox_Get_Size.Text);

            if(CheckIntTextValidation(size_text.Text))
            {
                if(size_text.Text.Length <=3)

                {
                    Debug.WriteLine("HEREEE");

                    e.Handled = true;
                    the_size_w= size_text.Text;

                }
                else
                {
                    size_text.Text = the_size_w;
                    e.Handled = false;
                }
            }
            else
            {
                size_text.Text= string.Empty;    
                e.Handled= false;
            }

        }

        private void load_line(int line)
        {
            int iterator=1;
         
                foreach (var item in dialogs)
                {
                    if (iterator == line)
                    {
                        Set_UI(item, iterator);
                        break;
                    }
                    iterator++;
                }
  
        }
        private void Set_UI(GameDialogue dialog,int line_)
        {
           
            combo_emo.SelectedItem = dialog.emotion;
            action_textbox.Text = dialog.action_name;
            int itt = 0;
            foreach(var item in combo_speakers.Items)
            {
                Debug.WriteLine(item);
                Debug.WriteLine($"{item} == {dialog.speaker} ? {item.Equals(dialog.speaker)}");
                Debug.WriteLine($"Types: {item.GetType()} vs {dialog.speaker.GetType()}");
                Debug.WriteLine($"Len: {item.ToString().Length} vs {dialog.speaker.Length}");
                
                if (item.ToString().Equals(dialog.speaker))
                {
                    Debug.WriteLine("Same");
                    combo_speakers.SelectedIndex= itt;
                }
                itt++;
            }
          
            Debug.WriteLine(dialog.speaker);
            if (dialog.isLeft)
                left_radio.IsChecked = true;
            else
                right_radio.IsChecked = true;
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

            
            BranchManager(line_);
            combo_branch.SelectedItem = dialog.branch_id;
            AddChoicesToListView(dialog.multiple_choice);
            //CheckForBranchs();

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
                   
                    dialogs.RemoveAt(thisLine - 1);
                  
                    Set_UI(new GameDialogue(), thisLine);

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

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Dialogue files (*.json)|*.json"; ;
            dialog.DefaultDirectory = FILE_PATH;
            dialog.ShowDialog();
            dialog.AddExtension= true;
       
            //string fileNameToOpen=FILE_PATH+ "\\" + name_file_textbox.Text+".json";
            string path = dialog.FileName;
            if (File.Exists(path))
            {
                string json=File.ReadAllText(path);   
                dialogs.Clear();
                var obj=JsonSerializer.Deserialize(json, typeof(DialogueToJson));
                var ss = (DialogueToJson) obj;
                if(ss.dialogs == null)
                {
                    MessageBox.Show( "File is Not Dialogue Format!");
                    return;
                    
                }
                dialogs = ss.dialogs;
                load_line(thisLine);
            }
            else
            {
                Directory.CreateDirectory(FILE_PATH);
               
            }
                
        }
        private void set_style(object sender, RoutedEventArgs e)
        {
            

            string color_s = set_style_color();
            string speed_s = set_style_text_speed();
            string size_s = set_style_text_size();
            string bold_s = set_style_bold();
            string italic_s = set_style_italic();
            string animation_s = set_style_animation();
        
            if(text.SelectionLength >0)
            
            if (!color_s.Equals("") || !speed_s.Equals("") || !bold_s.Equals("") ||
                !italic_s.Equals("") ||!animation_s.Equals(""))
            {
                string st = text.SelectionStart.ToString();
                string end_s = (text.SelectionStart + (text.SelectionLength - 1)).ToString();
                string Header = "Style for " + st + " --- " + end_s;
                string tmp = text.Text;
                Debug.WriteLine("start and end="+st + ":"+ end_s);   
                string selction_TTEXT = tmp.Substring(text.SelectionStart, (text.SelectionLength ));
                string message = "Selection Line :\n" + selction_TTEXT + "\n\nStyles :" + color_s + speed_s + size_s+
                    bold_s + italic_s+ animation_s;

                MessageBox.Show(message, Header);
            }

        }
        string set_style_animation()
        {
            if (!text_animatation.Equals("normal"))
            {

                DStyle dialogueAnimation = new DStyle();
                dialogueAnimation.style_type = "animation";
                dialogueAnimation.start = text.SelectionStart;
                dialogueAnimation.end = text.SelectionStart + (text.SelectionLength - 1);
                dialogueAnimation.data = text_animatation;
                DialogueStyleModel objst = new DialogueStyleModel();
                objst.style = dialogueAnimation;
                dialogueStyles.Add(objst);
               
                normal_radio_btn.IsChecked = true;
                text_animatation = "normal";

                return " ("+ dialogueAnimation.data + ") ";
            }
            return "";
        }
        string set_style_italic()
        {
            if ((bool)IsItalic_checkbox.IsChecked)
            {
                DStyle dialogueItalic = new DStyle();
                dialogueItalic.style_type = "italic";
                dialogueItalic.start = text.SelectionStart;
                dialogueItalic.end = text.SelectionStart + (text.SelectionLength - 1);
                dialogueItalic.data = "italic";
                DialogueStyleModel objst = new DialogueStyleModel();
                objst.style = dialogueItalic;
                dialogueStyles.Add(objst);
                IsItalic_checkbox.IsChecked = false;
                return " (Italic) ";
            }
            return "";
        }
        string set_style_bold()
        {
            if ((bool)IsBold_checkbox.IsChecked)
            {
                    DStyle dialogueBold = new DStyle();
                     dialogueBold.style_type = "bold";
                     dialogueBold.start = text.SelectionStart;
                     dialogueBold.end = text.SelectionStart + (text.SelectionLength - 1);
                     dialogueBold.data = "bold";
                     DialogueStyleModel objst = new DialogueStyleModel();
                     objst.style = dialogueBold;
                     dialogueStyles.Add(objst);
                     IsBold_checkbox.IsChecked = false;
                return " (Bold) ";

            }
            return "";
        }
        string set_style_color()
        {
            if (!color_text.Text.Equals(""))
            {
                bool s = ColorValidation.TryParse(color_text.Text);
                if (s)
                {


                    DStyle dialogueColor = new DStyle();
                    dialogueColor.style_type = "color";
                    dialogueColor.start = text.SelectionStart;
                    dialogueColor.end = text.SelectionStart + (text.SelectionLength - 1);
                    dialogueColor.data = color_text.Text.ToLower();
                    DialogueStyleModel objst = new DialogueStyleModel();
                    objst.style = dialogueColor;
                    dialogueStyles.Add(objst);
                    
                    color_text.Text = "";
                    return " (Color Set To: "+ dialogueColor.data+") ";
                }
                else
                {

                    MessageBox.Show("Color is not Valid");
                }
             
            }
            return "";
        }
        string set_style_text_speed()
        {
            if (!speed_text.Text.Equals(""))
            {

                if (CheckSpeedTextValidation(speed_text.Text))
                {
                    DStyle dialoguespeed = new DStyle();
                    dialoguespeed.style_type = "speed";
                    dialoguespeed.start = text.SelectionStart;
                    dialoguespeed.end = text.SelectionStart + (text.SelectionLength - 1);
                    dialoguespeed.data = speed_text.Text;
                    DialogueStyleModel objst = new DialogueStyleModel();
                    objst.style = dialoguespeed;
                    dialogueStyles.Add(objst);
                    speed_text.Text = "";
                    return " (Speed Change to : "+ dialoguespeed.data+") ";
                }

            }
            return "";
        }
        string set_style_text_size()
        {
            if (!size_text.Text.Equals(""))
            {

                if (CheckIntTextValidation(size_text.Text))
                {
                    DStyle dialoguesize = new DStyle();
                    dialoguesize.style_type = "size";
                    dialoguesize.start = text.SelectionStart;
                    dialoguesize.end = text.SelectionStart + (text.SelectionLength - 1);
                    dialoguesize.data = size_text.Text;
                    DialogueStyleModel objst = new DialogueStyleModel();
                    objst.style = dialoguesize;
                    dialogueStyles.Add(objst);
                    size_text.Text = "";
                    return " (Size Change to : " + dialoguesize.data + ") ";
                }

            }
            return "";
        }


        bool CheckSpeedTextValidation(string text)
        {
            try
            {
                float.Parse(text);

                return true;
            }
            catch
            {
                MessageBox.Show("Please insert float number");
                return false;
            }
        }
        bool CheckIntTextValidation(string text)
        {
            if (text.Equals("")) return true;
            try
            {
                int.Parse(text);

                return true;
            }
            catch
            {
                MessageBox.Show("Please insert number");
                return false;
            }
        }

        private void Reset_Style_Click(object sender, RoutedEventArgs e)
        {
            dialogueStyles.Clear(); 
        }

        private void Open_Folder_Click(object sender, RoutedEventArgs e)
        {
            var path =  FILE_PATH;
            Process.Start("explorer.exe",path);
        }
        private void Open_Folder_para_Click(object sender, RoutedEventArgs e)
        {
            var path = FILE_PATH_PARA;
            Process.Start("explorer.exe",path);
        }



        void add_to_ListView_choice(MultipleChoiceModel choice)
        {
            //ListBox_Chooices.Items.Add(choice.choice_head + "  |  " + choice.branch_name);
            BranchComponentModel BCM = new BranchComponentModel { BranchID = choice.branch_name, BranchName = choice.choice_head };
            BranchComponentViewModel BCVM = new BranchComponentViewModel();
            BCVM.BranchComponentBindig = BCM;
            BCVM.OnBranchCommand += HandleCommand;
            MainWindowViewModel.Instance.BranchesCollection.Add(BCVM);
            // Save To Multi Choice
        }

        private void HandleCommand(string  command, string branch_id)
        {
                switch(command)
            {
                case "delete":
                    MultipleChoiceModel del_choice = new MultipleChoiceModel();
                    if(dialogs.Count<= thisLine - 1)
                    {
                        MainWindowViewModel.Instance.DeleteFromBranches(branch_id);
                        MultipleChoiceModel del_choice_tm = new MultipleChoiceModel();
                        foreach (var dd in dialogueChoiuces)
                        {
                            if (dd.branch_name.Equals(branch_id))
                                del_choice_tm = dd;
                        }
                        dialogueChoiuces.Remove(del_choice_tm);
                        return;
                    }
                    foreach (var choi in dialogs[thisLine-1].multiple_choice)
                        if (choi.branch_name.Equals(branch_id))
                            del_choice = choi;
                    //Debug.WriteLine(del_choice.branch_name);
                    if (del_choice != null) dialogs[thisLine - 1].multiple_choice.Remove(del_choice);

                    MainWindowViewModel.Instance.DeleteFromBranches(branch_id);
                    break;
            }
        }



        private void next_line_btn_Click(object sender, RoutedEventArgs e)
        {
            ClearChoiceUI();
            Debug.WriteLine("this line:"+thisLine);
            if (thisLine  != dialogs.Count)
            {

                load_line(thisLine + 1);
                thisLine++;
                prev_btn.IsEnabled = true;
            }
            else
            {


                if (Submit_Dialog())
                {
                    if (thisLine - 1 == dialogs.Count)
                    {
                        text.Text = "";
                    }
                    else
                    {
                        thisLine++;
                        prev_btn.IsEnabled = true;
                        load_line(thisLine);
                    }

                }

                next_line_btn.IsEnabled = false;
            }
        }

        void OpenEmptyUI()
        {
           thisLine++;
           Set_UI(null, thisLine);
        }
            

        private void prev_btn_Click(object sender, RoutedEventArgs e)
        {
            ClearChoiceUI();
            if (thisLine != 1)
            {


                load_line(thisLine - 1);
                thisLine--;
                next_line_btn.IsEnabled = true;
                //CheckForBranchs();
            }
            else
            {
                prev_btn.IsEnabled = false;
                //MessageBox.Show("there is no previous line");
            }
        }

        void ClearChoiceUI()
        {
            dialogueChoiuces.Clear();
            choice_text.Clear();
            choice_branch.Clear();  
        }

        private void ChoiceKeyDown_branch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                if (choice_text.Text != "")
                {
                    if (choice_branch.Text == "")
                    {

                        choice_branch.Text = Make_Name(thisLine, dialogueChoiuces.Count + 1);
                    }
                    if (!SaveChoice(choice_text.Text, choice_branch.Text))
                    {
                        MessageBox.Show("Branch name is unique ");
                    }
                    else
                    {
                    }
                }
            }
        }



        private void ChoiceKeyDown_text(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {

                if (choice_text.Text != "")
                {
                    if (choice_branch.Text != "")
                    {
                        // save
                    }
                    else
                    {
                        // It has be unique 
                        
                        if(dialogs.Count>=thisLine)
                        {
                            if (dialogs[thisLine - 1].multiple_choice.Count>0)
                            choice_branch.Text = Make_Name(thisLine, dialogs[thisLine - 1].multiple_choice.Count + 1);
                            else
                                choice_branch.Text = Make_Name(thisLine, dialogueChoiuces.Count + 1);
                        }
                           
                        else 
                            choice_branch.Text = Make_Name(thisLine, dialogueChoiuces.Count + 1);
                     
                    }

                    if (!SaveChoice(choice_text.Text, choice_branch.Text))
                    {
                        MessageBox.Show("Branch name is unique ");
                    }
                }
                else
                {

                }

            }

        }



        private string Make_Name(int lineNumber, int i)
        {
            string ln = lineNumber.ToString();
            string i_n = i.ToString();
            string b_name = "branch_" + ln + "_" + i_n;
            return b_name;
        }


        private bool SaveChoice(string text, string branch)
        {
            //ShowListTome();
            foreach (var ch in choices_all)
            {
                foreach (var item in ch)
                {
                    if (item.branch_name.Equals(branch))
                    {
                        return false;
                    }
                }
            }

            MultipleChoiceModel choice = new MultipleChoiceModel();
            choice.choice_head = text;
            choice.branch_name = branch;
            dialogueChoiuces.Add(choice);
            add_to_ListView_choice(choice);
            choices_all.Add(dialogueChoiuces);
            choice_branch.Text = "";
            choice_text.Text = "";
            return true;
        }

        void DeleteListViewBranch()
        {
            MainWindowViewModel.Instance.BranchesCollection.Clear();
        }

        void AddChoicesToListView(List<MultipleChoiceModel> choices)
        {
            DeleteListViewBranch();
            foreach (var choice in choices)
            {
                add_to_ListView_choice((MultipleChoiceModel)choice);
               
            }
        }

        void BranchListViewManager()
        {

        }
   
        void BranchManager(int line)
        {
            // Branch Added to list 
            // So now i want to delete the Current Branch
            var branches= combo_branch.Items;
            List<string> list_branches = new List<string>();
            combo_branch.SelectedItem = null;
            combo_branch.Items.Clear();
            //Debug.WriteLine(line);
            list_branches=Get_Previous_Branches(line);
            foreach (var b in list_branches)
                combo_branch.Items.Add(b);
            
        }


        List<string> Get_Previous_Branches(int line)
        {
            List<string> list_branches= new List<string>{ main_brach };
          
            for (int i=0;i<line-1;i++)
            {
                if(dialogs[i].multiple_choice.Count>0)
                {
                    string b_tmp="";
                    List<string> list_branches_tmp = new List<string>();
                    //We Have Branch in this line 
                    foreach (var b in list_branches)
                    {
                        if (b.Equals(dialogs[i].branch_id))
                        {
                            //Remove the branch and add new ones 
                            //list_branches.Remove(b);
                            b_tmp = b;
                            foreach (var mc in dialogs[i].multiple_choice)
                            {
                                list_branches_tmp.Add(mc.branch_name);
                            }

                        }
                    }
                    int index = -1;
                    for(int j=0;j< list_branches.Count;j++)
                    {
                        if (list_branches[j].Equals(b_tmp))
                            index = j;
                    }
                    try
                    {
                        //Debug.WriteLine(index);
                        list_branches.RemoveAt(index);
                        list_branches.InsertRange(index, list_branches_tmp);
                    }
                    catch {
                       
                        MessageBox.Show("Branch must be fixed");
                       
                    }
            
                }
            }


            //foreach(var t in list_branches)
            //    Debug.WriteLine(t);
            return list_branches;   
        }



        AboutWindow win_about ;
        private void closedB(object sender, EventArgs e)
        {
            if(win_about!=null)
            win_about.Close();
            Process.GetCurrentProcess().Kill();
        }

        private void about_window(object sender, RoutedEventArgs e)
        {

            //AboutWindow.Instance.Show();
             //win_about = new AboutWindow();

            if (win_about==null) win_about=new AboutWindow();
            if (!win_about.IsLoaded)
                win_about = new AboutWindow();

            win_about.Show();

           


        }


        
    }
}