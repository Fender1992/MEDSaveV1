using System.Collections.ObjectModel;

namespace MEDSaveV1.Views;

public partial class ServicesPage : ContentPage
{
    public ObservableCollection<Service> Services { get; set; }

    public ServicesPage()
    {
        InitializeComponent();
        Services = new ObservableCollection<Service>
            {
                new Service { Name = "X-ray" },
                new Service { Name = "MRI" },
                new Service { Name = "CT" },
                new Service { Name = "Ultrasound" },
                new Service { Name = "Well Woman" },
                new Service { Name = "Rehydration" },
                new Service { Name = "Physical" },
                // Add other services as needed
            };

        BindingContext = this;
    }


    private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Service selectedService)
        {

            DisplayAlert("Service Selected", $"You selected {selectedService.Name}", "OK");
            //Navigation.PushAsync(new MapPage());
        }
    }

}

public class Service
{
    public string Name { get; set; }
}