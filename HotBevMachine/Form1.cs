namespace HotBevMachine;

public partial class Form1 : Form
{
    // Form1 - HotBev Machine -- M�quina de bebidas quentes

    /* Index
     * - Vari�veis Globais
     * - ctor
     * - Custom Methods
     * - Controls' Methods
     */
    
    // --- Vari�veis Globais ---
    
    FormInserirMoedas? _frmIM; // Refer�ncia a n�vel global porque � usada em 2+ m�todos
    int _custo = 0; // Custo da(s) bebida(s) pedida(s)
    int _troco; // Refer�ncia para a diferen�a entre Quantia e _custo
    int _totalBevs = 0; // N�mero total de bebidas pedidas
    int[] _bevs = { 0, 0, 0, 0 }; // N�mero total pedido cada bebida
    
    public int Quantia { get; set; }
    // ^ Vari�vel correspondente ao total de dinheiro inserido pelo user
    public int[] Moedas { get; set; }
    // ^ Array das moedas que o user inseriu, para o caso de cancelar o pedido
    
    // --- ctor ---

    public Form1()
    {
        InitializeComponent();

        // � necess�rio dar um valor inicial ao array
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
        // Se couber � direita do Form1, fica l�
        // Se n�o, fica � esquerda

        Point p = new(Right, Top); // Ponto � direita do Form1
        int r = Screen.GetWorkingArea(p).Right; // Abcissa direita do ecr� vis�vel

        if (_frmIM == null) return;

        // Verifica se o FormInserirMoedas cabe horizontalmente no ecr� � direita do Form1
        // Se n�o, fica � esquerda do Form1
        if (Right + _frmIM.Width < r)
            _frmIM.Location = p;
        else
            _frmIM.Location = new(Left - _frmIM.Width, Top);
    }

    public void Quantizar(int q, int[] m)
    {
        // Atualiza a Quantia e o valor mostrado na label
        Quantia = q;
        lblQuantia.Text = string.Format("{0:F2} �", (float)q / 100);

        _troco = Quantia - _custo;

        // Atualiza as moedas inseridas
        Moedas = m;
    }

    public void Destrocar()
    {
        // Atualiza o valor de _troco e a label que o mostra

        // t � uma vari�vel necess�ria para poder reverter o valor mostrado na label
        int t = _troco = Quantia - _custo;

        // Se ainda n�o foi pedida bebida, n�o atualiza a label
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
        lblFaltroco.Text = string.Format("{0},{1:D2} �", t / 100, t % 100);
    }

    public void OnFormInserirMoedas_Closed()
    {
        cbbInserirMoedas.Checked = false;
    }

    #endregion


    // --- Controls' Methods ---
    /* Sub-Index
     * Bot�o Inserir Moedas
     * Bot�es de sele��o de Bebidas
     * Bot�o Confirmar
     * Bot�o Reset
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
        // Clique do Bot�o de bebida

        Button _btn = (Button) sender;
        int qtd; // quantidade da bebida escolhida, a determinar mais abaixo

        // M�ximo de 5 bebidas de cada vez
        if (_totalBevs == 5)
        {
            MessageBox.Show("S� pode comprar 5 bebidas de uma vez.");
            return;
        }

        // Bot�o de bebida escolhida fica verde
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

        // Mostra a quantidade atual no bot�o da bebida
        _btn.Text = qtd.ToString();

        _troco = Quantia - _custo;
        _totalBevs++;

        // Atualiza a label que mostra o troco ou dinheiro em falta
        Destrocar();
    }

    private void btnConfirmar_Click(object sender, EventArgs e)
    {
        // Confirmar o pedido
        // - Vari�veis
        // - Verifica��es
        // - Determina��o do texto final a ser mostrado na MessageBox
        // - Detalhes finais

        // --- Vari�veis ---

        // Vari�veis auxiliares para determinar as moedas de troco
        int t = _troco, c;
        int[] moedas = { 200, 100, 50, 20, 10, 5 };

        // Strings para definir o texto final a ser mostrado na MessageBox
        string[] bebidas = { "Ch�", "Caf�", "Cappuccino", "Chocolate" };
        string quantas = "";
        string stroco = "";
        string final = "";

        // --- Verifica��es ---

        // � preciso escolher pelo menos uma bebida
        if (_totalBevs < 1)
        {
            MessageBox.Show("� preciso escolher uma bebida.");
            return;
        }

        // � preciso inserir dinheiro suficiente para o pedido
        if(_custo > Quantia)
        {
            string falta = string.Format("{0},{1:D2}", -t/100, -t%100);
            MessageBox.Show($"N�o introduziu dinheiro suficiente.\n" +
                $"Falta {falta} �.");
            return;
        }

        // --- Determina��o do texto final ---

        // Indica��o das bebidas pedidas e entregues
        if(_totalBevs == 1)
        {
            // Se for s� uma bebida: "Aqui tem o seu ..."
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

        // Indica��o de troco e quantas moedas
        if (_troco > 0)
        {
            stroco = string.Format("O troco � {0},{1:D2} �.", _troco / 100, _troco % 100);

            // Determina��o das moedas de troco
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
                        final += $"\n-> {c} moeda{(c > 1 ? 's' : "")} de {x / 100} �.";
                    else
                        final += $"\n-> {c} moeda{(c > 1 ? 's' : "")} de {x} c�ntimos.";
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

        // Prepara a m�quina para um novo pedido
        btnReset_Click(sender, e);
    }

    private void btnReset_Click(object sender, EventArgs e)
    {
        // Prepara a m�quina para um novo pedido

        // Se o pedido foi cancelado, devolve as moedas inseridas
        if (sender == btnReset && Quantia > 0)
        {
            string devol = "Devolu��o:";
            int[] moedas = { 2, 1, 50, 20, 10, 5};
            
            for (int i = 0; i< 6; i++)
            {
                if (Moedas[i] > 0)
                {
                    devol +=
                        $"\n-> {Moedas[i]} moeda{( Moedas[i] > 1 ? 's':"")} de " +
                        $"{moedas[i]}{(moedas[i] < 5 ? '�' : " c�ntimos")}.";
                }
            }

            MessageBox.Show(devol);
        }

        // Reset �s vari�veis
        _custo = 0;
        _totalBevs = 0;
        Array.Clear(_bevs);
        Quantia = 0;
        Array.Clear(Moedas);

        // Se o FormInserirMoedas estiver aberto, faz reset � Quantia
        _frmIM?.ClearQuantia();

        // Reset aos Controlos
        lblQuantia.Text = "0,00 �";
        lblFaltroca.ForeColor = Color.DarkRed;
        lblFaltroca.Text = "Em falta:";
        lblFaltroco.Text = "---";

        // Reset aos Bot�es das bebidas
        foreach(Button b in panel4.Controls.OfType<Button>())
        {
            b.BackColor = Color.Maroon;
            b.FlatAppearance.MouseOverBackColor = Color.Chocolate;
            b.Text = "";
        }
    }

    #endregion

}