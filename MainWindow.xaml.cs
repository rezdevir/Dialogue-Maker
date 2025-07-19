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
    public bool isMain {  get; set; }   
    public string text {  get; set; }   
    public string ToJson()
    {
     return   JsonSerializer.Serialize(this);
    }
   
}

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
        bool isMain;
        List<GameDialog> dialogs=new List<GameDialog>();
        public MainWindow()
        {

            InitializeComponent();
            read_parameter_file();
        }

        private void read_parameter_file()
        {
            
            if(File.Exists("speakers.txt"))
            {
                var list = File.ReadAllLines("speakers.txt");
                foreach(var speaker in list) 
                    {
                    //list_speakers.Add(speaker);
                    combo_speakers.Items.Add(speaker);  
                    }
            }
            if (File.Exists("emotions.txt"))
            {
                var list = File.ReadAllLines("emotions.txt");
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
            
            if(combo_speakers.SelectedItem !=null && combo_emo.SelectedItem  != null 
                && !text.Text.Equals(""))
            {
            
            var dialog = new GameDialog();
            dialog.text = text.Text;    
            dialog.isMain = isMain;
            dialog.speaker = combo_speakers.SelectedItem.ToString();
            dialog.emotion = combo_emo.SelectedItem.ToString();

                //dialogs.Add(dialog);
                add_to_list(dialog,thisLine);

                Debug.WriteLine("Line Saved");
                text.Text = "";
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
                var path = name_file_textbox.Text + ".txt";
                FileStream file;
                //File.
                if (!File.Exists(path))
                {
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
            return false;
            
            }
   
        }
        
        private void main_radio_Checked(object sender, RoutedEventArgs e)
        {
            isMain=true;
            Debug.WriteLine(isMain); 
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            isMain=false;
            Debug.WriteLine(isMain);
        }

        private void prev_btn_Click(object sender, RoutedEventArgs e)
        {
            if (thisLine != 1)
            {


                load_line(thisLine - 1);
                thisLine--;
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
            combo_speakers.SelectedItem = dialog.speaker;
            main_radio.IsChecked = dialog.isMain;
            text.Text = dialog.text;

            n_lines.Text = "Line : " + (line_) .ToString();
            if(dialog.text!=null)
            {
                n_chars.Text = "Number of Characters " + dialog.text.Length.ToString() + "//150";
            }
            else
            {
                n_chars.Text = "Number of Characters " + 0.ToString() + "//150";
            }
          
            
        }
        private void next_line_btn_Click(object sender, RoutedEventArgs e)
        {
            if(thisLine-1 != dialogs.Count) 
                {
            load_line(thisLine+1);
            thisLine++;
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

        }



        private void save_file_btn_Click(object sender, RoutedEventArgs e)
        {
            if(save())
            {
                MessageBox.Show("Successfully saved");
            }
            else
            {
                MessageBox.Show("Not able to save file");
            }
        }

        private void load_file_btn_Click(object sender, RoutedEventArgs e)
        {

            string fileNameToOpen=name_file_textbox.Text+".txt";
            if (File.Exists(fileNameToOpen))
            {
                string json=File.ReadAllText(fileNameToOpen);   
                dialogs.Clear();
                var obj=JsonSerializer.Deserialize(json, typeof(Jsonn));
                var ss = (Jsonn) obj;
                dialogs = ss.dialogs;
                load_line(thisLine);
            } 
                
        }
    }
}