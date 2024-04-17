namespace HotBevMachine;

public partial class Form1 : Form
{
    // Form1 - HotBev Machine -- Máquina de bebidas quentes

    /* Index
     * - Variáveis Globais
     * - ctor
     * - Custom Methods
     * - Controls' Methods
     */
    
    // --- Variáveis Globais ---
    
    FormInserirMoedas? _frmIM; // Referência a nível global porque é usada em 2+ métodos
    int _custo = 0; // Custo da(s) bebida(s) pedida(s)
    int _troco; // Referência para a diferença entre Quantia e _custo
    int _totalBevs = 0; // Número total de bebidas pedidas
    int[] _bevs = { 0, 0, 0, 0 }; // Número total pedido cada bebida
    
    public int Quantia { get; set; }
    // ^ Variável correspondente ao total de dinheiro inserido pelo user
    public int[] Moedas { get; set; }
    // ^ Array das moedas que o user inseriu, para o caso de cancelar o pedido
    
    // --- ctor ---

    public Form1()
    {
        InitializeComponent();

        // É necessário dar um valor inicial ao array
        Moedas = new int[] { 0, 0, 0, 0, 0, 0 };
    }

    // --- Custom Methods ---
    /* Sub-Index
     * - LocateFormIM
     * - Quantizar
     * - Destrocar
     * - OnFormInserirMoedas_Closed
     */

    #region Custom Methods

    void LocateFormIM()
    {
        // Locar o FormInserirMoedas antes de o abrir
        // Se couber à direita do Form1, fica lá
        // Se não, fica à esquerda

        Point p = new(Right, Top); // Ponto à direita do Form1
        int r = Screen.GetWorkingArea(p).Right; // Abcissa direita do ecrã visível

        if (_frmIM == null) return;

        // Verifica se o FormInserirMoedas cabe horizontalmente no ecrã à direita do Form1
        // Se não, fica à esquerda do Form1
        if (Right + _frmIM.Width < r)
            _frmIM.Location = p;
        else
            _frmIM.Location = new(Left - _frmIM.Width, Top);
    }

    public void Quantizar(int q, int[] m)
    {
        // Atualiza a Quantia e o valor mostrado na label
        Quantia = q;
        lblQuantia.Text = string.Format("{0:F2} €", (float)q / 100);

        _troco = Quantia - _custo;

        // Atualiza as moedas inseridas
        Moedas = m;
    }

    public void Destrocar()
    {
        // Atualiza o valor de _troco e a label que o mostra

        // t é uma variável necessária para poder reverter o valor mostrado na label
        int t = _troco = Quantia - _custo;

        // Se ainda não foi pedida bebida, não atualiza a label
        if (_custo == 0) return;

        if (t > 0) // Se Quantia > _custo
        {
            lblFaltroca.Text = "Troco:";
            lblFaltroca.ForeColor = Color.Green;
        }
        else
        {
            t *= -1;
            lblFaltroca.Text = "Em falta:";
            lblFaltroca.ForeColor = Color.DarkRed;
        }

        // Atualiza o texto da label de troco ou dinheiro em falta
        lblFaltroco.Text = string.Format("{0},{1:D2} €", t / 100, t % 100);
    }

    public void OnFormInserirMoedas_Closed()
    {
        cbbInserirMoedas.Checked = false;
    }

    #endregion


    // --- Controls' Methods ---
    /* Sub-Index
     * Botão Inserir Moedas
     * Botões de seleção de Bebidas
     * Botão Confirmar
     * Botão Reset
     */

    #region Controls' Methods

    private void cbbInserirMoedas_CheckedChanged(object sender, EventArgs e)
    {
        // Abre e fecha o Form que permite Inserir Moedas

        if (cbbInserirMoedas.Checked)
        {
            _frmIM = new FormInserirMoedas(this);
            LocateFormIM();
            _frmIM.Show();
        }
        else
        {
            _frmIM?.Close();
        }
    }

    private void rbtBev_Click(object sender, EventArgs e)
    {
        // Clique do Botão de bebida

        Button _btn = (Button) sender;
        int qtd; // quantidade da bebida escolhida, a determinar mais abaixo

        // Máximo de 5 bebidas de cada vez
        if (_totalBevs == 5)
        {
            MessageBox.Show("Só pode comprar 5 bebidas de uma vez.");
            return;
        }

        // Botão de bebida escolhida fica verde
        _btn.BackColor = Color.Green;
        _btn.FlatAppearance.MouseOverBackColor = Color.LimeGreen;

        // Determina a bebida escolhida,
        // atualiza o _custo e determina a quantidade atual da bebida escolhida
        switch (_btn.Name)
        {
            case "rbtCha": _custo += 20; qtd = ++_bevs[0]; break;
            case "rbtCafe": _custo += 25; qtd = ++_bevs[1]; break;
            case "rbtCappuccino": _custo += 30; qtd = ++_bevs[2]; break;
            case "rbtChocolate": _custo += 35; qtd = ++_bevs[3]; break;
            default: return;
        }

        // Mostra a quantidade atual no botão da bebida
        _btn.Text = qtd.ToString();

        _troco = Quantia - _custo;
        _totalBevs++;

        // Atualiza a label que mostra o troco ou dinheiro em falta
        Destrocar();
    }

    private void btnConfirmar_Click(object sender, EventArgs e)
    {
        // Confirmar o pedido
        // - Variáveis
        // - Verificações
        // - Determinação do texto final a ser mostrado na MessageBox
        // - Detalhes finais

        // --- Variáveis ---

        // Variáveis auxiliares para determinar as moedas de troco
        int t = _troco, c;
        int[] moedas = { 200, 100, 50, 20, 10, 5 };

        // Strings para definir o texto final a ser mostrado na MessageBox
        string[] bebidas = { "Chá", "Café", "Cappuccino", "Chocolate" };
        string quantas = "";
        string stroco = "";
        string final = "";

        // --- Verificações ---

        // É preciso escolher pelo menos uma bebida
        if (_totalBevs < 1)
        {
            MessageBox.Show("É preciso escolher uma bebida.");
            return;
        }

        // É preciso inserir dinheiro suficiente para o pedido
        if(_custo > Quantia)
        {
            string falta = string.Format("{0},{1:D2}", -t/100, -t%100);
            MessageBox.Show($"Não introduziu dinheiro suficiente.\n" +
                $"Falta {falta} €.");
            return;
        }

        // --- Determinação do texto final ---

        // Indicação das bebidas pedidas e entregues
        if(_totalBevs == 1)
        {
            // Se for só uma bebida: "Aqui tem o seu ..."
            for(int i = 0; i< 4; i++)
            {
                if (_bevs[i] == 1)
                {
                    quantas = $"Aqui tem o seu {bebidas[i]}.";
                    break;
                }
            }
        }
        else
        {
            // Se for mais que uma bebida: "Aqui tem:\n{lista de bebidas}"
            quantas = $"Aqui tem:";

            // Acrescenta quantas, de cada bebida, a pessoa pediu
            for(int i = 0; i< 4; i++)
            {
                if (_bevs[i] > 0)
                    quantas += $"\n-> {_bevs[i]} {bebidas[i]}{(_bevs[i] > 1 ? 's' : "")}.";
            }
        }

        // Indicação de troco e quantas moedas
        if (_troco > 0)
        {
            stroco = string.Format("O troco é {0},{1:D2} €.", _troco / 100, _troco % 100);

            // Determinação das moedas de troco
            foreach (int x in moedas)
            {
                // Quantidade de cada moeda
                c = 0;
                while (t >= x)
                {
                    t -= x;
                    c++;
                }

                // Texto que indica a quantidade de cada moeda
                if (c > 0)
                {
                    if (x > 99)
                        final += $"\n-> {c} moeda{(c > 1 ? 's' : "")} de {x / 100} €.";
                    else
                        final += $"\n-> {c} moeda{(c > 1 ? 's' : "")} de {x} cêntimos.";
                }
            }
        }

        // --- Detalhes finais ---

        // Se o FormInserirMoedas ainda estiver aberto, fecha-o
        _frmIM?.Close();

        // Mostra o texto final
        MessageBox.Show(
            $"{quantas}\n" +
            $"{stroco}" +
            $"{final}"
            );

        // Prepara a máquina para um novo pedido
        btnReset_Click(sender, e);
    }

    private void btnReset_Click(object sender, EventArgs e)
    {
        // Prepara a máquina para um novo pedido

        // Se o pedido foi cancelado, devolve as moedas inseridas
        if (sender == btnReset && Quantia > 0)
        {
            string devol = "Devolução:";
            int[] moedas = { 2, 1, 50, 20, 10, 5};
            
            for (int i = 0; i< 6; i++)
            {
                if (Moedas[i] > 0)
                {
                    devol +=
                        $"\n-> {Moedas[i]} moeda{( Moedas[i] > 1 ? 's':"")} de " +
                        $"{moedas[i]}{(moedas[i] < 5 ? '€' : " cêntimos")}.";
                }
            }

            MessageBox.Show(devol);
        }

        // Reset às variáveis
        _custo = 0;
        _totalBevs = 0;
        Array.Clear(_bevs);
        Quantia = 0;
        Array.Clear(Moedas);

        // Se o FormInserirMoedas estiver aberto, faz reset à Quantia
        _frmIM?.ClearQuantia();

        // Reset aos Controlos
        lblQuantia.Text = "0,00 €";
        lblFaltroca.ForeColor = Color.DarkRed;
        lblFaltroca.Text = "Em falta:";
        lblFaltroco.Text = "---";

        // Reset aos Botões das bebidas
        foreach(Button b in panel4.Controls.OfType<Button>())
        {
            b.BackColor = Color.Maroon;
            b.FlatAppearance.MouseOverBackColor = Color.Chocolate;
            b.Text = "";
        }
    }

    #endregion

}