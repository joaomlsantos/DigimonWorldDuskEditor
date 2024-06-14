using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

public class MainForm : Form
{
    private ComboBox areaComboBox;
    private ComboBox valueComboBox;
    private Button saveButton;
    private BinaryFileService binaryFileService;
    private MappingService mappingService;
    private List<OffsetAddress> offsetAddresses;
    private List<ValueMapping> valueMappings;

    public MainForm()
    {
        binaryFileService = new BinaryFileService("C:/Workspace/digimon_stuffs/Digimon World - Dusk (USA) - Copy.nds");
        mappingService = new MappingService("./Data/offsets.txt", "./Data/values.txt");
        offsetAddresses = mappingService.GetOffsetAddresses();
        valueMappings = mappingService.GetValueMappings();

        InitializeComponents();
    }

    private void InitializeComponents()
    {
        areaComboBox = new ComboBox { Dock = DockStyle.Top };
        valueComboBox = new ComboBox { Dock = DockStyle.Top };
        saveButton = new Button { Text = "Save", Dock = DockStyle.Top };

        areaComboBox.SelectedIndexChanged += AreaComboBox_SelectedIndexChanged;
        saveButton.Click += SaveButton_Click;

        foreach (var offsetAddress in offsetAddresses)
        {
            areaComboBox.Items.Add(offsetAddress.AreaName);
        }

        foreach (var valueMapping in valueMappings)
        {
            valueComboBox.Items.Add(valueMapping.ValueName);
        }

        Controls.Add(saveButton);
        Controls.Add(valueComboBox);
        Controls.Add(areaComboBox);
    }

    private void AreaComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedArea = areaComboBox.SelectedItem.ToString();
        var offset = offsetAddresses.FirstOrDefault(oa => oa.AreaName == selectedArea)?.Offset ?? 0;
        var currentData = binaryFileService.ReadBytes(offset, 2);

        var currentMapping = valueMappings.FirstOrDefault(vm => vm.HexValue.SequenceEqual(currentData));
        if (currentMapping != null)
        {
            valueComboBox.SelectedItem = currentMapping.ValueName;
        }
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
        var selectedArea = areaComboBox.SelectedItem.ToString();
        var offset = offsetAddresses.FirstOrDefault(oa => oa.AreaName == selectedArea)?.Offset ?? 0;
        var selectedValue = valueComboBox.SelectedItem.ToString();
        var newData = valueMappings.FirstOrDefault(vm => vm.ValueName == selectedValue)?.HexValue;

        if (newData != null)
        {
            binaryFileService.WriteBytes(offset, newData);
            MessageBox.Show("Data saved successfully.");
        }
        else
        {
            MessageBox.Show("Invalid value selected.");
        }
    }
}