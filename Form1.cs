using System;
using System.Collections.Generic;
using System.IO;              // Для работы с файлами
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Practika
{
    public partial class Form1 : Form
    {
        private readonly HttpClient httpClient = new HttpClient();
        private List<Country> countries;

        public Form1()
        {
            InitializeComponent();
            this.Shown += Form1_Shown;
        }

        // Загрузка списка стран при появлении формы
        private async void Form1_Shown(object sender, EventArgs e)
        {
            await LoadCountriesAsync();
        }

        // Метод для загрузки стран из API Nager.Date
        private async Task LoadCountriesAsync()
        {
            try
            {
                string url = "https://date.nager.at/api/v3/AvailableCountries";
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();

                var array = JArray.Parse(json);
                countries = new List<Country>();

                foreach (var item in array)
                {
                    string name = item["name"]?.ToString();
                    string code = item["countryCode"]?.ToString();
                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(code))
                    {
                        countries.Add(new Country { Name = name, Code = code });
                    }
                }

                if (countries.Count == 0)
                {
                    MessageBox.Show("Не удалось загрузить ни одной страны.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                comboBoxCountry.DataSource = countries;
                comboBoxCountry.DisplayMember = "Name";
                comboBoxCountry.ValueMember = "Code";
                comboBoxCountry.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки стран: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик нажатия кнопки "Получить"
        private async void buttonGetHolidays_Click(object sender, EventArgs e)
        {
            // Проверка: выбрана ли страна
            if (comboBoxCountry.SelectedItem == null)
            {
                MessageBox.Show("Выберите страну из списка.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedCountry = (Country)comboBoxCountry.SelectedItem;
            string countryCode = selectedCountry.Code;

            if (string.IsNullOrEmpty(countryCode))
            {
                MessageBox.Show($"У выбранной страны '{selectedCountry.Name}' отсутствует код.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int year = dateTimePickerYear.Value.Year;

            // Проверка года
            if (year < 1900 || year > 2100)
            {
                MessageBox.Show("Пожалуйста, выберите год от 1900 до 2100.", "Некорректный год", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Блокировка кнопки и изменение текста
            buttonGetHolidays.Enabled = false;
            buttonGetHolidays.Text = "Загрузка...";

            listBoxHolidays.Items.Clear();
            listBoxHolidays.Items.Add("Загрузка...");

            string url = $"https://date.nager.at/api/v3/PublicHolidays/{year}/{countryCode}";

            try
            {
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Ошибка {response.StatusCode} при запросе:\n{url}", "Ошибка API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    listBoxHolidays.Items.Clear();
                    listBoxHolidays.Items.Add("Не удалось получить праздники.");
                    return;
                }

                string json = await response.Content.ReadAsStringAsync();
                var holidays = JsonConvert.DeserializeObject<List<Holiday>>(json);

                listBoxHolidays.Items.Clear();

                if (holidays == null || holidays.Count == 0)
                {
                    listBoxHolidays.Items.Add($"Для страны '{selectedCountry.Name}' в {year} году праздников не найдено.");
                    return;
                }

                listBoxHolidays.Items.Add($"Найдено праздников: {holidays.Count}");
                listBoxHolidays.Items.Add("-----------------------------");

                foreach (var h in holidays)
                {
                    listBoxHolidays.Items.Add($"{h.Date:dd.MM.yyyy} - {h.LocalName} ({h.Name})");
                }
            }
            catch (Exception ex)
            {
                listBoxHolidays.Items.Clear();
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonGetHolidays.Enabled = true;
                buttonGetHolidays.Text = "Получить";
            }
        }

        // НОВОЕ: Сохранение списка праздников в текстовый файл
        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Проверяем, есть ли данные для сохранения
            if (listBoxHolidays.Items.Count == 0 || listBoxHolidays.Items[0].ToString() == "Загрузка...")
            {
                MessageBox.Show("Нет данных для сохранения. Сначала получите список праздников.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Диалог выбора места сохранения
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.FileName = $"Holidays_{comboBoxCountry.Text}_{dateTimePickerYear.Value.Year}.txt";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Сохраняем все строки из ListBox в файл
                        string[] lines = new string[listBoxHolidays.Items.Count];
                        for (int i = 0; i < listBoxHolidays.Items.Count; i++)
                        {
                            lines[i] = listBoxHolidays.Items[i].ToString();
                        }
                        File.WriteAllLines(saveFileDialog.FileName, lines);

                        MessageBox.Show($"Файл успешно сохранён:\n{saveFileDialog.FileName}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }

    public class Country
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class Holiday
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }
        [JsonProperty("localName")]
        public string LocalName { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}