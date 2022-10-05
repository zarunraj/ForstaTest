namespace QuizClient.UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Uri quizServiceUri = new Uri("http://localhost:62910/");
        private async void Form1_Load(object sender, EventArgs e)
        {
            HttpClient httpClient = new HttpClient();
            QuizClient quizClient = new QuizClient(quizServiceUri, httpClient);
            var quizes = await quizClient.GetQuizzesAsync(CancellationToken.None);
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox1.Visible = false;
            foreach (var item in quizes.Value)
            {
                Button btn = new Button();
                btn.Text = item.Title;
                btn.Name = item.Id.ToString();
                btn.Click += Btn_Click;
                flowLayoutPanel1.Controls.Add(btn);
            }
        }

        private async void Btn_Click(object? sender, EventArgs e)
        {
            var btn = sender as Button;
            var id = Int32.Parse(btn.Name);

            HttpClient httpClient = new HttpClient();
            QuizClient quizClient = new QuizClient(quizServiceUri, httpClient);
            var quizResponse = await quizClient.GetQuizAsync(id, CancellationToken.None);
            var quiz = quizResponse.Value;

            flowLayoutPanel1.Visible = false;
            flowLayoutPanel2.Visible = true;
            groupBox1.Visible = true;
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Text = quiz.Title;
            groupBox1.Controls.Clear();
            groupBox1.Name =quiz.Id.ToString();
            for (int i=0; i<quiz.Questions.Count();i++)
            {
                var q = quiz.Questions.ElementAt(i);
                var grp = new GroupBox() { Text = q.Text, Name =$"{q.Id}_{q.CorrectAnswerId}" };
                

                for (int j = 0; j < q.Answers.Count(); j++)
                {
                    var ans = q.Answers.ElementAt(j);
                    var rdo = new RadioButton() { Text =ans.Text, Name  =ans.Id.ToString()};
                    rdo.Location = new Point(10, 20 * (j + 1));
                    grp.Controls.Add(rdo);
                }
                flowLayoutPanel2.Controls.Add(grp);
            }

        }
    }
}