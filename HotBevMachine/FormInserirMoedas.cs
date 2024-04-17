namespace HotBevMachine;

public partial class FormInserirMoedas : Form
{
    // FormInserirMoedas -> Insere o dinheiro para pagar as bebidas

    int _quantia; // Quantia inserida, a mostrar na label
    int[] _moedas = new int[6]; // Moedas inseridas para atualizar o array do Form1
    Form1 _form1; // Referência para poder atualizar o Form1

    public FormInserirMoedas(Form1 f)
    {
        InitializeComponent();

        _form1 = f;
        _quantia = f.Quantia;

        // Atualiza a label com a quantia já inserida
        lblQuantia.Text = string.Format("{0},{1:D2} €", _quantia / 100, _quantia % 100);
    }

    public void ClearQuantia()
    {
        // Reset nas variáveis e atualiza a label da Quantia inserida
        _quantia = 0;
        Array.Clear(_moedas);
        lblQuantia.Text = "0,00 €";
    }

    private void rbtEc_Click(object sender, EventArgs e)
    {
        // Botão Inserir Moeda

        // Variáveis auxiliares
        int coin, mIndex;

        // Determina a moeda inserida e o índice para o array Moedas do Form1
        switch (((Button)sender).Text)
        {
            case "5c": coin = 5; mIndex = 5; break;
            case "10c": coin = 10; mIndex = 4; break;
            case "20c": coin = 20; mIndex = 3; break;
            case "50c": coin = 50; mIndex = 2; break;
            case "1€": coin = 100; mIndex = 1; break;
            case "2€": coin = 200; mIndex = 0; break;
            default:
                MessageBox.Show("Houve um erro. A moeda não foi reconhecida.");
                return;
        }

        // Limite -- A máquina apenas recebe um máximo de 2 €
        if (_quantia + coin > 200)
        {
            // Devolve a moeda inserida

            // Verifica se a moeda é de Euros ou de Cêntimos
            if (coin > 99) coin /= 100;

            // Devolve
            MessageBox.Show(
                $"A máquina não aceita mais de 2€.\n" +
                $"Devolução: moeda de {coin}" +
                $"{(coin < 5 ? "€" : " cêntimos")}."
                );

            return;
        }

        // Atualiza os valores neste Form
        _quantia += coin;
        _moedas[mIndex]++;

        // Atualiza os valores no Form1
        _form1.Quantizar(_quantia, _moedas);
        _form1.Destrocar();

        // Atualiza a label da Quantia inserida
        lblQuantia.Text = string.Format("{0},{1:D2} €", _quantia / 100, _quantia % 100);
    }

    private void FormInserirMoedas_FormClosed(object sender, FormClosedEventArgs e)
    {
        _form1.OnFormInserirMoedas_Closed();
    }
}
