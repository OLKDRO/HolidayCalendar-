using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Practika   // Убедитесь, что пространство имён совпадает с вашим проектом
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

            // ---- ИЗМЕНЕНИЕ 1: Проверка года ----
            if (year < 1900 || year > 2100)
            {
                MessageBox.Show("Пожалуйста, выберите год от 1900 до 2100.", "Некорректный год", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ---- ИЗМЕНЕНИЕ 2: Блокировка кнопки и изменение текста ----
            buttonGetHolidays.Enabled = false;
            buttonGetHolidays.Text = "Загрузка...";

            listBoxHolidays.Items.Clear();
            listBoxHolidays.Items.Add("Загрузка...");

            // Формируем URL для запроса к API
            string url = $"https://date.nager.at/api/v3/PublicHolidays/{year}/{countryCode}";

            try
            {
                // Отправляем GET-запрос к API
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Ошибка {response.StatusCode} при запросе:\n{url}", "Ошибка API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    listBoxHolidays.Items.Clear();
                    listBoxHolidays.Items.Add("Не удалось получить праздники.");
                    return;
                }

                string json = await response.Content.ReadAsStringAsync();
                // Десериализуем JSON в список праздников
                var holidays = JsonConvert.DeserializeObject<List<Holiday>>(json);

                listBoxHolidays.Items.Clear();

                if (holidays == null || holidays.Count == 0)
                {
                    listBoxHolidays.Items.Add($"Для страны '{selectedCountry.Name}' в {year} году праздников не найдено.");
                    return;
                }

                // ---- ИЗМЕНЕНИЕ 5: Вывод количества праздников ----
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
                // ---- ИЗМЕНЕНИЕ 2 (окончание): Разблокировка кнопки ----
                buttonGetHolidays.Enabled = true;
                buttonGetHolidays.Text = "Получить";
            }
        }
    }

    // Класс для хранения данных о стране
    public class Country
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    // Класс для десериализации праздника
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