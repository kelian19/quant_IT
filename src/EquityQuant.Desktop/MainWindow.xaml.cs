namespace EquityQuant.Desktop;
public partial class MainWindow : System.Windows.Window { public MainWindow(){InitializeComponent(); DataContext=new PricingViewModel();} }
