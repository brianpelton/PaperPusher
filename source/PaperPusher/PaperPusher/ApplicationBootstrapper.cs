using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using PaperPusher.ViewModels;

namespace PaperPusher
{
    public class ApplicationBootstrapper : BootstrapperBase
    {
        #region [ Fields ]

        private CompositionContainer _container;

        #endregion

        #region [ Constructors ]

        public ApplicationBootstrapper()
        {
            Initialize();
        }

        #endregion

        #region [ Methods ]

        protected override void BuildUp(object instance)
        {
            _container.SatisfyImportsOnce(instance);
        }

        protected override void Configure()
        {
            var catalog = new AggregateCatalog(
                AssemblySource.Instance.Select(x => new AssemblyCatalog(x)));

            _container = new CompositionContainer(catalog);

            var batch = new CompositionBatch();
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(_container);
            batch.AddExportedValue(catalog);

            _container.Compose(batch);
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            var contract = string.IsNullOrEmpty(key)
                ? AttributedModelServices.GetContractName(serviceType)
                : key;
            var exports = _container.GetExportedValues<object>(contract).ToList();

            if (exports.Any())
                return exports.First();

            throw new Exception($"Could not locate any instances of contract {contract}.");
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            DisplayRootViewFor<MainViewModel>();
        }

        #endregion
    }
}