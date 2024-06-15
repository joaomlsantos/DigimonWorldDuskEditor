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
        private ListBox areaListBox;
        private Panel valuePanel;
        private Button saveButton;
        private BinaryFileService binaryFileService;
        private MappingService mappingService;
        private List<OffsetAddress> offsetAddresses;
        private List<ValueMapping> valueMappings;
        private Dictionary<string, List<(byte[], long)>> preloadedValues;
        private Dictionary<ComboBox, long> valueComboBoxOffsets;
    
        public MainForm()
        {
            binaryFileService = new BinaryFileService("C:/Workspace/digimon_stuffs/Digimon World - Dusk (USA).nds");
            mappingService = new MappingService("C:/Workspace/digimon_stuffs/DigimonWorldDuskEditor/Data/locations.txt", "C:/Workspace/digimon_stuffs/DigimonWorldDuskEditor/Data/digimon.txt");
            offsetAddresses = mappingService.GetOffsetAddresses();

            valueMappings = mappingService.GetValueMappings();

            preloadedValues = new Dictionary<string, List<(byte[], long)>>();
            valueComboBoxOffsets = new Dictionary<ComboBox, long>();

            PreloadValues();
            InitializeComponents();
        }
    
        private void PreloadValues()
        {
            var interval = 0x18;
            foreach (var offsetAddress in offsetAddresses)
            {
                var offset = offsetAddress.Offset;
                var values = binaryFileService.ReadValuesAtIntervals(offset + 0x10, interval, 0x00, 0xFF);
                preloadedValues[offsetAddress.AreaName] = values;
            }
        }

        private void InitializeComponents()
        {
            this.Text = "Digimon World Dusk Editor";
            this.Width = 750;
            this.Height = 500;

            areaListBox = new ListBox { Dock = DockStyle.Left, Width = 250 };
            valuePanel = new Panel { Dock = DockStyle.Fill };
            saveButton = new Button { Text = "Save", Dock = DockStyle.Bottom, Height = 40, Width = 100 };

            areaListBox.SelectedIndexChanged += AreaListBox_SelectedIndexChanged;
            saveButton.Click += SaveButton_Click;
    
            foreach (var offsetAddress in offsetAddresses)
            {
                areaListBox.Items.Add(offsetAddress.AreaName);
            }
    
            Controls.Add(saveButton);
            Controls.Add(valuePanel);
            Controls.Add(areaListBox);

            //CreateComboBoxes();
        }


/*
        private void AreaListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedArea = areaListBox.SelectedItem.ToString();
            var offset = offsetAddresses.FirstOrDefault(oa => oa.AreaName == selectedArea)?.Offset ?? 0;

            // New interval and termination logic
            var interval = 0x18; // 24 in decimal
            var values = binaryFileService.ReadValuesAtIntervals(offset + 0x10, interval, 0x00, 0xFF);
            
            LoadValues(values);
        }
*/


        private void AreaListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedArea = areaListBox.SelectedItem.ToString();
            if (preloadedValues.ContainsKey(selectedArea))
            {
                LoadValues(preloadedValues[selectedArea]);
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

                foreach (var valueMapping in valueMappings)
                {
                    comboBox.Items.Add(valueMapping.ValueName);
                }
                if (mapping != null)
                {
                    comboBox.SelectedItem = mapping.ValueName;
                }
                else
                {
                    comboBox.Items.Add(hexValue);
                    comboBox.SelectedItem = hexValue;
                }
                valueComboBoxOffsets[comboBox] = offset;
                comboBoxes.Add(comboBox);
            }
            int comboBoxIter = 0;
            foreach (var comboBox in comboBoxes)
            {
                if(comboBoxIter != 0 && comboBoxIter % 2 == 0)
                    yPos += 40;
                comboBox.Top = yPos;
                comboBox.Left = 10 + 250 * (comboBoxIter % 2);
                valuePanel.Controls.Add(comboBox);
                comboBoxIter += 1;
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
    }
}