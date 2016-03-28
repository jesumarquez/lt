using TechTalk.SpecFlow;
using UI.Web.Test.Page;

namespace UI.Web.Test.Specs
{
    class OrdenesSteps :BaseStep
    {
        [Given(@"Estoy logeado")]
        public void GivenEstoyLogeado()
        {
            var page = new LoginPage(Driver);
            SetCurrentPage(page.LoginAs("jose.gutierrez", "jagj77*").UsePerfil());
        }

        [When(@"Voy a la pagina ordenes")]
        public void WhenVoyALaPaginaOrdenes()
        {
            var page = GetCurrentPage<MainPage>();
            var ordenesPage = page.Ordenes();
            SetCurrentPage(ordenesPage);
        }

        [Then(@"veo el filtro de '(.*)'")]
        public void ThenVeoElFiltorDe(string nombreFiltro)
        {
            GetCurrentPage<OrdenesPage>().ExisteFiltro(nombreFiltro);
        }
        [Then(@"veo los filtros")]
        public void ThenVeoLosFiltros(Table table)
        {
            foreach (var row in table.Rows)
            {
                GetCurrentPage<OrdenesPage>().ExisteFiltro(row[0]);
            }
        }

        [Then(@"Veo una grilla con las columnas:")]
        public void ThenVeoUnaGrillaConLasColumnas(Table table)
        {
            foreach (var row in table.Rows)
            {
                GetCurrentPage<OrdenesPage>().ExisteColumna(row[0]);
            }
        }

        [When(@"filtro '(.*)' con valor '(.*)'")]
        public void WhenIngresoConValor(string filtro, string valor)
        {
            GetCurrentPage<OrdenesPage>().SetFiltro(filtro, valor);
        }

        [When(@"presiono buscar")]
        public void WhenBuscar()
        {
            GetCurrentPage<OrdenesPage>().Buscar();
        }


    }
}
