using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaobabRidge
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();

            SetupAnimalGrid();
            LoadAnimals();
        }


        private readonly string filePath =
        Path.Combine(AppContext.BaseDirectory, "animals.txt");

        // Configures the grid columns and prevents direct editing.
        private void SetupAnimalGrid()
        {
            dgvAnimals.Columns.Clear();
            dgvAnimals.AutoGenerateColumns = false;

            dgvAnimals.ReadOnly = true;
            dgvAnimals.AllowUserToAddRows = false;
            dgvAnimals.AllowUserToDeleteRows = false;
            dgvAnimals.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvAnimals.MultiSelect = false;
            dgvAnimals.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            AddGridColumn("AnimalID", "Animal ID");
            AddGridColumn("Name", "Name");
            AddGridColumn("Species", "Species");
            AddGridColumn("Age", "Age");
            AddGridColumn("RecoveryScore", "Recovery Score");
        }

        // Adds a read-only column with the correct heading.
        private void AddGridColumn(string property, string heading)
        {
            dgvAnimals.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = property,
                Name = property,
                HeaderText = heading,
                ReadOnly = true
            });
        }

        // 5. File reading method
        private List<Animal> ReadAnimals(out int skippedLines)
        {
            List<Animal> animals = new List<Animal>();
            skippedLines = 0;

            if (!File.Exists(filePath))
            {
                using (File.Create(filePath)) { }
                return animals;
            }

            string[] lines = File.ReadAllLines(
                filePath, Encoding.UTF8);

            foreach (string line in lines)
            {
                string[] fields = line.Split('|');

                if (fields.Length != 7)
                {
                    skippedLines++;
                    continue;
                }

                string id = fields[0].Trim().ToUpperInvariant();

                bool validID = Regex.IsMatch(
                    id, @"^WR-[0-9]{4}$");

                bool validAge = int.TryParse(
                    fields[3], out int age);

                bool validScore = int.TryParse(
                    fields[4], out int score);

                if (!validID || !validAge || !validScore ||
                    age < 0 || age > 100 ||
                    score < 0 || score > 100)
                {
                    skippedLines++;
                    continue;
                }

                Animal animal = new Animal
                {
                    AnimalID = id,
                    Name = fields[1],
                    Species = fields[2],
                    Age = age,
                    RecoveryScore = score
                };

                animals.Add(animal);
            }

            return animals;
        }

        // 6. Grid loading method
        public void LoadAnimals()
        {
            try
            {
                int skippedLines;

                List<Animal> animals =
                    ReadAnimals(out skippedLines);

                // Clear the old data before displaying new records.
                dgvAnimals.DataSource = null;
                dgvAnimals.DataSource = animals;

                dgvAnimals.ClearSelection();

                if (skippedLines > 0)
                {
                    MessageBox.Show(
                        skippedLines +
                        " invalid record(s) were skipped.",
                        "File Loading",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    "Permission denied when accessing animals.txt.",
                    "File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (IOException)
            {
                MessageBox.Show(
                    "Unable to access animals.txt. " +
                    "The file may be in use.",
                    "File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
       
        // Deletes the selected animal from the file.
        public void DeleteAnimal(string animalID, string animalName)
        {
            DialogResult answer = MessageBox.Show(
                "Delete " + animalID + " (" + animalName +
                ")? This cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer != DialogResult.Yes) return;

            try
            {
                List<string> lines = File.ReadAllLines(filePath, Encoding.UTF8).ToList();

                int removed = lines.RemoveAll(line =>
                    line.Split('|')[0].Trim().Equals(
                        animalID, StringComparison.OrdinalIgnoreCase));

                if (removed == 0)
                {
                    MessageBox.Show("Animal not found.");
                    LoadAnimals();
                    return;
                }

                File.WriteAllLines(filePath, lines, Encoding.UTF8);

                LoadAnimals();

                MessageBox.Show("Animal deleted successfully.");
            }
            catch (IOException)
            {
                MessageBox.Show("Animals.txt could not be read or saved. The file may be open in another program.");
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Permission denied.");
            }
        }

        // Refreshes the animal grid.
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadAnimals();
        }

        // Deletes the selected animal after confirmation.
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAnimals.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an animal first.");
                return;
            }

            string id = dgvAnimals.SelectedRows[0]
                .Cells["AnimalID"].Value.ToString();

            string name = dgvAnimals.SelectedRows[0]
                .Cells["Name"].Value.ToString();

            DeleteAnimal(id, name);
        }

    }
}

