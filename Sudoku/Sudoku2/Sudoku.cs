using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku2
{
    public partial class Form1 : Form
    {
        const int n = 3;
        public int[,] map = new int[n * n, n * n];
        public Button[,] buttons = new Button[n * n, n * n];
        const int sizeButton = 50;
        private int selectedNumber = 1;
        private bool pencilMode = false;
        public int lives = 3;

        // Обробник події натискання на кнопку числа для вибору числа.
        //number - Обране число на кнопці.
        private void OnNumberButtonClick(int number)
        {
            selectedNumber = number;
        }

        // Встановлення стилів для кнопки.
        //button - Кнопка, для якої встановлюються стилі.
        private void SetButtonStyles(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = this.BackColor; 
            button.FlatAppearance.BorderColor = Color.Gray; 
            button.FlatAppearance.BorderSize = 1; 
        }
        public Form1()
        {
            // Ініціалізація компонентів форми
            InitializeComponent();
            // Генерація ігрового поля "Судоку"
            GenerateMap();
            // Ініціалізація кнопок з номерами
            InitializeNumberButtons();
        }
        //Метод для генерації та виведення ігрового поля
        public void GenerateMap()
        {
            // Ініціалізація масивів та кнопок
            for (int i = 0; i < n * n; i++)
            {
                for (int j = 0; j < n * n; j++)
                {
                    map[i, j] = (i * n + i / n + j) % (n * n) + 1;
                    buttons[i, j] = new Button();
                }
            }
            // Випадкове перемішування мапи
            Random r = new Random();
            for (int i = 0; i < 40; i++)
            {
                ShuffleMap(r.Next(0, 5));
            }
            // Створення відображення мапи на формі
            CreateMap();
            // Приховування деяких комірок
            HideCells();
        }

        // Приховує випадкові комірки на ігровій дошці.
        public void HideCells()
        {
            // Кількість комірок для приховування
            int N = 40;
            // Випадкове приховування комірок
            Random r = new Random();
            while (N > 0)
            {
                // Перебір усіх комірок
                for (int i = 0; i < n * n; i++)
                {
                    for (int j = 0; j < n * n; j++)
                    {
                        // Перевірка, чи має комірка текст
                        if (!string.IsNullOrEmpty(buttons[i, j].Text))
                        {
                            // Випадкове приховування комірки
                            int a = r.Next(0, 3);
                            buttons[i, j].Text = a == 0 ? "" : buttons[i, j].Text;
                            buttons[i, j].Enabled = a == 0 ? true : false;
                            // Зменшення лічильника залишених прихованих комірок
                            if (a == 0)
                                N--;
                            // Переривання, якщо вже приховано необхідну кількість комірок
                            if (N <= 0)
                                break;
                        }
                    }
                    // Переривання, якщо вже приховано необхідну кількість комірок
                    if (N <= 0)
                        break;
                }
            }
        }
        // Перемішує карту гри за вибраним методом, який визначається параметром i.
        public void ShuffleMap(int i)
        {
            switch (i)
            {
                case 0:
                    MatrixTransposition();
                    break;
                case 1:
                    SwapRowsInBlock();
                    break;
                case 2:
                    SwapColumnsInBlock();
                    break;
                case 3:
                    SwapBlocksInRow();
                    break;
                case 4:
                    SwapBlocksInColumn();
                    break;
                default:
                    MatrixTransposition();
                    break;
            }
        }
        // Обмін блоків у випадковому стовпці на ігровій дошці.
        public void SwapBlocksInColumn()
        {
            Random r = new Random();

            // Вибір випадкових блоків 
            var block1 = r.Next(0, n);
            var block2 = r.Next(0, n);

            while (block1 == block2)
                block2 = r.Next(0, n);

            // Перетворення індексів блоків у відповідні стовпці
            block1 *= n;
            block2 *= n;
            for (int i = 0; i < n * n; i++)
            {
                var k = block2;
                for (int j = block1; j < block1 + n; j++)
                {
                    var temp = map[i, j];
                    map[i, j] = map[i, k];
                    map[i, k] = temp;
                    k++;
                }
            }
        }
        // Обмін блоків у випадковому рядку на ігровій дошці.
        public void SwapBlocksInRow()
        {
            Random r = new Random();
            var block1 = r.Next(0, n);
            var block2 = r.Next(0, n);
            while (block1 == block2)
                block2 = r.Next(0, n);
            block1 *= n;
            block2 *= n;
            for (int i = 0; i < n * n; i++)
            {
                var k = block2;
                for (int j = block1; j < block1 + n; j++)
                {
                    var temp = map[j, i];
                    map[j, i] = map[k, i];
                    map[k, i] = temp;
                    k++;
                }
            }
        }
        // Обмін рядків у межах випадковому блоку на ігровій дошці.
        public void SwapRowsInBlock()
        {
            Random r = new Random();

            // Вибір випадкового блоку та його рядків для обміну
            var block = r.Next(0, n);
            var row1 = r.Next(0, n);
            var line1 = block * n + row1;

            // Вибір другого рядка для обміну, переконуючись, щоб він був відмінний від першого
            var row2 = r.Next(0, n);
            while (row1 == row2)
                row2 = r.Next(0, n);
            var line2 = block * n + row2;
            for (int i = 0; i < n * n; i++)
            {
                var temp = map[line1, i];
                map[line1, i] = map[line2, i];
                map[line2, i] = temp;
            }
        }
        // Обмін стовпців у межах випадковому блоку на ігровій дошці.
        public void SwapColumnsInBlock()
        {
            Random r = new Random();

            // Вибір випадкового блоку та його стовпців для обміну
            var block = r.Next(0, n);
            var row1 = r.Next(0, n);
            var line1 = block * n + row1;

            // Вибір другого стовпця для обміну, переконуючись, щоб він був відмінний від першого
            var row2 = r.Next(0, n);
            while (row1 == row2)
                row2 = r.Next(0, n);
            var line2 = block * n + row2;
            for (int i = 0; i < n * n; i++)
            {
                var temp = map[i, line1];
                map[i, line1] = map[i, line2];
                map[i, line2] = temp;
            }
        }
        // Транспонує матрицю гри.
        public void MatrixTransposition()
        {
            int[,] tMap = new int[n * n, n * n];
            for (int i = 0; i < n * n; i++)
            {
                for (int j = 0; j < n * n; j++)
                {
                    tMap[i, j] = map[j, i];
                }
            }
            map = tMap;
        }
        // Створює відображення ігрової дошки на формі.
        public void CreateMap()
        {
            // Створення кнопок для кожної комірки матриці
            int blockMargini = 0;
            for (int i = 0; i < n * n; i++)
            {
                int blockMarginj = 0;
                for (int j = 0; j < n * n; j++)
                {
                    Button button = new Button();
                    buttons[i, j] = button;

                    // Налаштування розміру та тексту кнопки
                    button.Size = new Size(sizeButton, sizeButton);
                    button.Text = map[i, j].ToString();

                    // Налаштування події кліку на кнопку
                    button.Click += OnCellPressed;

                    // Розміщення кнопки на формі з врахуванням відступів між блоками
                    button.Location = new Point(j * sizeButton + blockMarginj, i * sizeButton + blockMargini);

                    // Оновлення відступу між блоками для стовпця
                    var columMardgin = (j + 1) % 3 == 0 ? blockMarginj += 5 : blockMarginj += 0;

                    // Налаштування стилів кнопки
                    SetButtonStyles(button);
                    this.Controls.Add(button);
                }
                // Оновлення відступу між блоками для рядка
                var rowMardgin = (i + 1) % 3 == 0 ? blockMargini += 5 : blockMargini += 0;
            }
        }

        // Обробник події натискання на клітинку гравцем.
        //sender - Об'єкт, який викликав подію.
        //e - Аргументи події.
        private void OnCellPressed(object sender, EventArgs e)
        {
            Button pressedButton = sender as Button;

            int rowIndex = -1;
            int colIndex = -1;
            for (int i = 0; i < n * n; i++)
            {
                for (int j = 0; j < n * n; j++)
                {
                    if (buttons[i, j] == pressedButton)
                    {
                        rowIndex = i;
                        colIndex = j;
                        break;
                    }
                }
            }
            // Виклик відповідного методу залежно від режиму гри
            if (pencilMode)
            {
                PencilModeOn(pressedButton);
            }
            else
            {
                // Встановлення тексту на кнопці та перевірка значення клітинки
                pressedButton.Text = selectedNumber.ToString();
                CheckCellValue(rowIndex, colIndex);
            }
        }

        // Обробка події при ввімкненому режимі олівця.
        //pressedButton - Кнопка, яка викликала подію.
        public void PencilModeOn(Button pressedButton)
        {
            string buttonText = pressedButton.Text;

            if (string.IsNullOrWhiteSpace(buttonText))
            {

                pressedButton.Text = selectedNumber.ToString();
                pressedButton.ForeColor = Color.DarkCyan;
            }
            else
            {
                List<string> numbers = buttonText.Split(' ').ToList();

                if (numbers.Contains(selectedNumber.ToString()))
                {
                    numbers.Remove(selectedNumber.ToString());
                }
                else if (numbers.Count >= 1)
                {
                    numbers.Add(selectedNumber.ToString());
                }
                // Об'єднання чисел у рядок та встановлення нового тексту кнопки
                pressedButton.Text = string.Join(" ", numbers);
                pressedButton.ForeColor = Color.DarkCyan;
            }
        }

        private List<Button> numberButtons;

        // Ініціалізація кнопок для вибору чисел.
        private void InitializeNumberButtons()
        {
            numberButtons = new List<Button> { button1, button2, button3,
                button4, button5, button6, button7, button8, button9 };

            foreach (var button in numberButtons)
            {
                int number = int.Parse(button.Text);
                button.Click += (sender, e) => OnNumberButtonClick(number);
            }
        }

        // Зменшення кількості життів та відображення зображень відповідно до кількості життів.
        //lives - Кількість залишених життів.
        private void ReductionOfLives(int lives)
        {
            if (lives == 0)
                pictureBox1.Image = Properties.Resources.brokenHeart;
            if (lives == 1)
                pictureBox2.Image = Properties.Resources.brokenHeart;
            if (lives == 2)
                pictureBox3.Image = Properties.Resources.brokenHeart;
        }

        // Перевірка значення клітинки та виконання відповідних дій.
        //row - Індекс рядка клітинки.
        //col - Індекс стовпця клітинки.
        private void CheckCellValue(int row, int col)
        {
            var btnText = buttons[row, col].Text;
            // Перевірка, чи текст кнопки співпадає з правильним значенням
            if (btnText != map[row, col].ToString())
            {
                buttons[row, col].BackColor = Color.Red;

                // Зменшення кількості життів та відображення зображень сердець
                lives--;
                ReductionOfLives(lives);

                // Перевірка, чи гравець втратив усі життя
                if (lives == 0)
                {
                    MessageBox.Show("У Вас не залишилось життів! Ви програли!");
                    RestartTheGame();
                }
            }
            else
            {
                buttons[row, col].BackColor = this.BackColor;
                buttons[row, col].Enabled = false;

                // Перевірка, чи головоломка розв'язана
                CheckIsSolutionCorrect();
            }
        }

        // Перевірка, чи головоломка вирішена правильно.
        private void CheckIsSolutionCorrect()
        {
            for (int i = 0; i < n * n; i++)
            {
                for (int j = 0; j < n * n; j++)
                {
                    var btn = buttons[i, j].Text;
                    if (btn != map[i, j].ToString())
                    {
                        return;
                    }
                }
            }
            // Перевірка, чи головоломка вирішена правильно.
            MessageBox.Show("Вітаю! Вирішено правильно!");
            RestartTheGame();
        }

        // Перезапуск гри, очищення інтерфейсу та генерація нової головоломки.
        public void RestartTheGame()
        {
            for (int i = 0; i < n * n; i++)
            {
                for (int j = 0; j < n * n; j++)
                {
                    this.Controls.Remove(buttons[i, j]);
                }
            }
            // Скидання кількості життів та відображення сердець
            lives = 3;
            pictureBox1.Image = Properties.Resources.Heart;
            pictureBox2.Image = Properties.Resources.Heart;
            pictureBox3.Image = Properties.Resources.Heart;
            selectedNumber = 1;
            GenerateMap();
        }
        // Зміна режиму гри між олівцем та ручкою.
        private void PenMode_Click(object sender, EventArgs e)
        {
            pencilMode = !pencilMode;
            PenMode.Text = pencilMode ? "Режим: Олівець" : "Режим: Ручка";
        }
    }
}
