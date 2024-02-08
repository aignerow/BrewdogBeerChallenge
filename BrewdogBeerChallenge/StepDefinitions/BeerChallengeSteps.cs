using BrewdogBeerChallenge.Helpers;
using BrewdogBeerChallenge.Model;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using RestSharp;
using TechTalk.SpecFlow;
using Method = RestSharp.Method;

namespace BrewdogBeerChallenge.StepDefinitions
{
    [Binding]
    public class BeerChallengeSteps
    {
        private readonly RestClient _client;
        private RestRequest _restRequest;
        private RestResponse _restResponse;

        public BeerChallengeSteps() => _client = new RestClient(Constants.AppUrl);

        [When(@"I have list of beers brewed after (.*)")]
        public void WhenIHaveListOfBeersBrewedAfter(string brewedAfter)
        {
            _restRequest = new RestRequest(Constants.BeerEndPoint, Method.Get);
            _restRequest.AddQueryParameter(Constants.BrewedAfter, brewedAfter);

            _restResponse = _client.Execute<List<Beer>>(_restRequest);
        }

        [Then(@"beers abv value is a double over (.*)")]
        public void ThenBeersAbvValueIsADoubleOver(double abvValue)
        {
            List<Beer> ListOfBeers = GetListOfBeers();

            //below assertion will check all entities in the list at once.
            using (new AssertionScope())
            {
                foreach (var beer in ListOfBeers)
                {
                    beer.Abv.Should().BeOfType<double>($"beer with id {beer.Id} Abv should be double");
                    beer.Abv.Should().NotBe("", $"beer with id {beer.Id} Abv should not be empty string");
                    beer.Abv.Should().NotBeNull($"beer with id {beer.Id} Abv should not be null");
                    Convert.ToDouble(beer.Abv).Should().BeGreaterThan(abvValue, $"beer with id {beer.Id} Abv should be greater than {abvValue}");
                }
            }
            //If we'd deserialize abv to double type field, with current data, all beers would be properly deserialized,
            //but there would be no option of validation for empty string, being of double type and not null.
            //In case of any beer not having double as abv, this would cause failures during deserialization process.
            //This is the reason why I've decided to keep abv as object type for the sake of this challenge, however it
            //would be better to have it as double in the real world scenario. Especially that Deserialization of whole number 
            //to object type field is making it of type integer, which makes assertion BeOfType<double> fail in 3 cases.
        }

        [Then(@"beers names are a non empty strings")]
        public void ThenBeersNamesAreNonEmptyStrings()
        {
            List<Beer> ListOfBeers = GetListOfBeers();

            //below assertion will check all entities in the list at once.
            using (new AssertionScope())
            {
                foreach (var beer in ListOfBeers)
                {
                    beer.Name.Should().BeOfType<string>($"beer with id {beer.Id} Name should be string");                    
                    beer.Name.Should().NotBeNullOrEmpty($"beer with id {beer.Id} Name should not be null or empty");
                }
            }
        }

        [Then(@"beers ingredients are listed")]
        public void ThenBeersIngredientsAreListed()
        {
            List<Beer> ListOfBeers = GetListOfBeers();

            //below assertion will check all entities in the list at once.
            using (new AssertionScope())
            {
                foreach (var beer in ListOfBeers)
                {
                    beer.Ingredients.Should().NotBeNull($"beer with id {beer.Id} Ingredients should not be null");
                    beer.Ingredients.Malt.Should().NotBeNull($"beer with id {beer.Id} Malt should not be null");
                    beer.Ingredients.Hops.Should().NotBeNull($"beer with id {beer.Id} Hops should not be null");
                    beer.Ingredients.Yeast.Should().NotBeNullOrEmpty($"beer with id {beer.Id} Yeast should not be null or empty");
                }
            }
        }

        [Then(@"beers image has correct url format")]
        public void ThenBeersImageHasCorrectUrlFormat()
        {
            List<Beer> ListOfBeers = GetListOfBeers();

            //below assertion will check all entities in the list at once.
            using (new AssertionScope())
            {
                   foreach (var beer in ListOfBeers)
                {
                    beer.Image_url.Should().NotBeNullOrEmpty($"beer with id {beer.Id} Image_url should not be null or empty");
                    beer.Image_url.Should().MatchRegex(Constants.ImageUrlPattern, $"beer with id {beer.Id} Image_url should match url pattern");
                }
            }
        }

        private List<Beer> GetListOfBeers()
        {
            List<Beer> ListOfBeers = JsonConvert.DeserializeObject<List<Beer>>(_restResponse.Content);

            if (ListOfBeers == null || ListOfBeers.Count < 1)
            {
                throw new InvalidDataException("List of beers should have members");
            }

            return ListOfBeers;
        }
    }
}
