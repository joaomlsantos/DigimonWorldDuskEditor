using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DigimonWorldDuskEditor.Models;
using DigimonWorldDuskEditor.Services;
using System.Diagnostics;


namespace DigimonWorldDuskEditor.Forms
{
    public class MainForm : Form
    {
        private TabControl tabControl;
        private TabPage encountersTabPage;
        private TabPage evolutionsTabPage;
        //private ListBox areaListBox;
        //private Panel valuePanel;
        private Button saveButton;
        private MenuStrip menuStrip;
        private ToolStripMenuItem fileMenuItem;
        private ToolStripMenuItem openFileMenuItem;
        private string loadedRomPath;

        private BinaryFileService binaryFileService;
        private MappingService mappingService;
        public List<OffsetAddress> offsetAddresses;
        public List<ValueMapping> valueMappings;
        //public Dictionary<ComboBox, long> valueComboBoxOffsets;

        private EncounterEditor encounterEditor;
        private EvolutionEditor evolutionEditor;
    
        public MainForm()
        {
            //binaryFileService = new BinaryFileService("C:/Workspace/digimon_stuffs/Digimon World - Dusk (USA).nds");
            mappingService = new MappingService("C:/Workspace/digimon_stuffs/DigimonWorldDuskEditor/Data/locations.txt", "C:/Workspace/digimon_stuffs/DigimonWorldDuskEditor/Data/digimon.txt");
            offsetAddresses = mappingService.GetOffsetAddresses();

            valueMappings = mappingService.GetValueMappings();
            //valueComboBoxOffsets = new Dictionary<ComboBox, long>();

            //PreloadValues();
            InitializeComponents();
        }
    

        private void InitializeComponents()
        {
            this.Text = "Digimon World Dusk Encounter Editor";
            this.Width = 760;
            this.Height = 550;

            menuStrip = new MenuStrip();
            fileMenuItem = new ToolStripMenuItem("File");
            openFileMenuItem = new ToolStripMenuItem("Open...");
            openFileMenuItem.Click += OpenFileMenuItem_Click;
            fileMenuItem.DropDownItems.Add(openFileMenuItem);
            menuStrip.Items.Add(fileMenuItem);

            //areaListBox = new ListBox { Dock = DockStyle.Left, Width = 250, Enabled = false };
            //valuePanel = new Panel { Dock = DockStyle.Fill, Enabled = false };
            saveButton = new Button { Text = "Save", Dock = DockStyle.Bottom, Height = 40, Width = 100, Enabled = false };
            saveButton.Click += SaveButton_Click;
    
            //areaListBox.SelectedIndexChanged += AreaListBox_SelectedIndexChanged;
            
            //foreach (var offsetAddress in offsetAddresses)
            //{
            //    areaListBox.Items.Add(offsetAddress.AreaName);
            //}
            
            tabControl = new TabControl { Dock = DockStyle.Top, Height = 450 };
            encountersTabPage = new TabPage("Encounters");
            evolutionsTabPage = new TabPage("Evolutions");

            encounterEditor = new EncounterEditor(offsetAddresses);
            evolutionEditor = new EvolutionEditor(offsetAddresses);

            encountersTabPage.Controls.Add(encounterEditor);
            evolutionsTabPage.Controls.Add(evolutionEditor);

            tabControl.TabPages.Add(encountersTabPage);
            tabControl.TabPages.Add(evolutionsTabPage);

    
            //Controls.Add(valuePanel);
            //Controls.Add(areaListBox);
            Controls.Add(tabControl);
            Controls.Add(saveButton);
            Controls.Add(menuStrip);

        }

        private void OpenFileMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "NDS files (*.nds)|*.nds|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFileName = openFileDialog.FileName;
                    binaryFileService = new BinaryFileService(selectedFileName);
                    encounterEditor.LoadData(binaryFileService, offsetAddresses);
                    //preloadedValues.Clear();
                    //PreloadValues();
                    
                    foreach (Control control in Controls)
                    {
                        control.Enabled = true;
                    }
                }
            }
        }


        /*

        private void AreaListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedArea = areaListBox.SelectedItem.ToString();
            if (preloadedValues.ContainsKey(selectedArea))
            {
                LoadValues(preloadedValues[selectedArea]);
            }
        }

        private void DigimonComboBox_DropDown(object sender, EventArgs e)
        {
            ComboBox curComboBox = (ComboBox)sender;
            curComboBox.Items.Clear();
            foreach (var valueMapping in valueMappings)
            {
                curComboBox.Items.Add(valueMapping.ValueName);
            }
        }

        private void LoadValues(List<(byte[], long)> values)
        {
            valuePanel.SuspendLayout();
            valuePanel.Controls.Clear();
            valueComboBoxOffsets.Clear();

            var comboBoxes = new List<ComboBox>();

            int yPos = 10;
            foreach (var (value, offset) in values)
            {
                var hexValue = Convert.ToHexString(value);
                var mapping = valueMappings.FirstOrDefault(v => Convert.ToHexString(v.HexValue) == hexValue);
                ComboBox comboBox = new ComboBox { Width = 200 };

                //foreach (var valueMapping in valueMappings)
                //{
                //    comboBox.Items.Add(valueMapping.ValueName);
                //}
                if (mapping != null)
                {
                    comboBox.Items.Add(mapping.ValueName);
                    comboBox.SelectedItem = mapping.ValueName;
                }
                else
                {
                    comboBox.Items.Add(hexValue);
                    comboBox.SelectedItem = hexValue;
                }

                comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                comboBox.AutoCompleteCustomSource.AddRange(valueMappings.Select(vm => vm.ValueName).ToArray());

                valueComboBoxOffsets[comboBox] = offset;
                comboBoxes.Add(comboBox);
            }
            int comboBoxIter = 0;
            foreach (var comboBox in comboBoxes)
            {
                if(comboBoxIter != 0 && comboBoxIter % 2 == 0)
                    yPos += 40;
                comboBox.Top = yPos;
                comboBox.Left = 30 + 240 * (comboBoxIter % 2);
                valuePanel.Controls.Add(comboBox);
                comboBoxIter += 1;
                comboBox.DropDown += DigimonComboBox_DropDown;
            }
            valuePanel.ResumeLayout();
        }


        private void SaveButton_Click(object sender, EventArgs e)
        {
            var newPreloadedValues = new List<(byte[], long)>();
            foreach (var comboBox in valueComboBoxOffsets.Keys)
            {
                var offset = valueComboBoxOffsets[comboBox];
                var selectedValue = comboBox.SelectedItem.ToString();
                var newData = valueMappings.FirstOrDefault(vm => vm.ValueName == selectedValue)?.HexValue;

                if (newData != null)
                {
                    binaryFileService.WriteBytes(offset, newData);
                }
                else
                {
                    MessageBox.Show("Invalid value selected.");
                }
                newPreloadedValues.Add((newData, offset));
            }
            preloadedValues[areaListBox.SelectedItem.ToString()] = newPreloadedValues;
            MessageBox.Show("Data saved successfully.");
        }
        */

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == encountersTabPage)
            {
                encounterEditor.SaveData(binaryFileService);
            }
            else if (tabControl.SelectedTab == evolutionsTabPage)
            {
                evolutionEditor.SaveData(binaryFileService);
            }
        }
    }
}