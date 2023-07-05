using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Customers;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Sales;
using Microsoft.Extensions.DependencyInjection;

namespace Litium.Accelerator.StateTransitions
{
    [RunAsSystem]
    public class SalesOrderFixture : ApplicationFixture
    {
        private CountryService _countryService;
        private ChannelService _channelService;
        private CurrencyService _currencyService;
        private FieldTemplateService _fieldTemplateService;
        private OrderService _orderService;
        private PaymentService _paymentService;
        private TransactionService _transactionService;
        private OrganizationService _organizationService;
        private PersonService _personService;
        private Currency Currency;
        private Country Country;
        private Channel Channel;
        private Person Person;
        public Sales.SalesOrder SalesOrder;

        public SalesOrderFixture()
        {
            Initialize();
            SalesOrder = CreateSalesOrder();
        }

        private Sales.SalesOrder CreateSalesOrder()
        {
            var salesOrder = CreateDefaultSalesOrder();
            salesOrder.Rows.Add(new SalesOrderRow()
            {
                SystemId = Guid.NewGuid(),
                Description = this.UniqueString(),
                Quantity = 1,
                TotalIncludingVat = 55,
                TotalExcludingVat = 50,
                TotalVat = 5,
                UnitPriceIncludingVat = 55,
                UnitPriceExcludingVat = 50,
                VatRate = 10,
                OrderRowType = OrderRowType.Product,
                ProductType = ProductType.DigitalGoods,
                ArticleNumber = this.UniqueString()
            });

            return salesOrder;
        }

        private void Initialize()
        {
            _personService = ServiceProvider.GetRequiredService<PersonService>();
            _currencyService = ServiceProvider.GetRequiredService<CurrencyService>();
            _countryService = ServiceProvider.GetRequiredService<CountryService>();
            _channelService = ServiceProvider.GetRequiredService<ChannelService>();
            var taxClassService = ServiceProvider.GetRequiredService<TaxClassService>();
            _fieldTemplateService = ServiceProvider.GetRequiredService<FieldTemplateService>();
            _paymentService = ServiceProvider.GetRequiredService<PaymentService>();
            _orderService = ServiceProvider.GetRequiredService<OrderService>();
            _transactionService = ServiceProvider.GetRequiredService<TransactionService>();
            _organizationService = ServiceProvider.GetRequiredService<OrganizationService>();

            Person = CreatePerson(_fieldTemplateService);
            Currency = CreateCurrency("SEK");
            Country = CreateCountry(Currency.SystemId);
            Channel = CreateChannel();
            Channel.CountryLinks.Add(new ChannelToCountryLink(Country.SystemId));
            _channelService.Update(Channel);

            var taxClass = CreateTaxClass(taxClassService);
            Country.TaxClassLinks.Add(new CountryToTaxClassLink(taxClass.SystemId) { VatRate = 0.12m });
            _countryService.Update(Country);
        }

        private TaxClass CreateTaxClass(TaxClassService taxClassService)
        {
            var taxClass = new TaxClass { Id = this.UniqueString() };
            taxClassService.Create(taxClass);
            AddDisposable(() => taxClassService.Delete(taxClass));

            return taxClass;
        }

        private Currency CreateCurrency(string currencyId)
        {
            var currency = _currencyService.GetAll().FirstOrDefault(x => x.Id == currencyId);
            if (currency == null)
            {
                currency = new Currency(currencyId);
                _currencyService.Create(currency);
                AddDisposable(() => _currencyService.Delete(currency));
            }

            return currency;
        }

        private Country CreateCountry(Guid currencySystemId)
        {
            var country = new Country(currencySystemId) { Id = this.UniqueString() };
            _countryService.Create(country);
            AddDisposable(() => _countryService.Delete(country));

            return country;
        }

        private Channel CreateChannel()
        {
            var channelFieldTemplate = new ChannelFieldTemplate(this.UniqueString());
            _fieldTemplateService.Create(channelFieldTemplate);
            AddDisposable(() => _fieldTemplateService.Delete(channelFieldTemplate));

            var channel = new Channel(channelFieldTemplate.SystemId) { Id = this.UniqueString() };
            _channelService.Create(channel);
            AddDisposable(() => _channelService.Delete(channel));

            return channel;
        }

        private Sales.SalesOrder CreateDefaultSalesOrder()
        {
            return new Sales.SalesOrder()
            {
                BaseCurrencyCode = Currency.Id,
                CurrencyCode = Currency.Id,
                CountryCode = Country.Id,
                ChannelSystemId = Channel.SystemId,
                OrderDate = DateTimeOffset.Now,
                CustomerInfo = new CustomerInfo()
                {
                    PersonSystemId = Person.SystemId,
                    CustomerNumber = Person.Id,
                    CustomerType = Person.OrganizationLinks.Count > 0 ? CustomerType.Organization : CustomerType.Person,
                    OrganizationSystemId = Person.OrganizationLinks?.FirstOrDefault()?.OrganizationSystemId
                },
                BillingAddress = new Sales.Address()
                {
                    FirstName = this.UniqueString(),
                    LastName = this.UniqueString(),
                    Address1 = "BillingAddress"
                },
                ExchangeRate = 1,
                OrderPaymentLinks = new List<OrderPaymentLink>()
            };
        }

        private Person CreatePerson(FieldTemplateService fieldTemplateService)
        {
            var personFieldTemplate = new PersonFieldTemplate(this.UniqueString());
            fieldTemplateService.Create(personFieldTemplate);
            AddDisposable(() => fieldTemplateService.Delete(personFieldTemplate));

            var person = new Person(personFieldTemplate.SystemId);
            _personService.Create(person);
            AddDisposable(() => _personService.Delete(person));

            return person;
        }

        public Transaction CreateTransaction(Guid paymentSystemId, TransactionType transactionType, TransactionResult transactionResult)
        {
            var transaction = new Transaction()
            {
                Id = this.UniqueString(),
                PaymentSystemId = paymentSystemId,
                TransactionType = transactionType,
                TransactionEnvironment = TransactionEnvironment.Test,
                TransactionResult = transactionResult,
                TransactionDate = DateTimeOffset.Now,
                TransactionReference1 = this.UniqueString(),
                TransactionReference2 = this.UniqueString(),
                LastModified = DateTimeOffset.Now,
                CurrencyCode = Currency.Id,
                PaymentOption = new ProviderOptionIdentifier(this.UniqueString(), this.UniqueString()),
                Rows = new List<TransactionRow>
                {
                    new TransactionRow
                    {
                        Id = this.UniqueString(),
                        RowType = TransactionRowType.PhysicalGoods,
                        UnitPriceExcludingVat = 50,
                        UnitPriceIncludingVat = 55,
                        Quantity = 1,
                        VatRate = 0.12m,
                        TotalExcludingVat = 50,
                        TotalIncludingVat = 55,
                        TotalVat = 5,
                    },
                    new TransactionRow
                    {
                        Id = this.UniqueString(),
                        RowType = TransactionRowType.ShippingFee,
                        UnitPriceExcludingVat = 0,
                        UnitPriceIncludingVat = 0,
                        Quantity = 1,
                        VatRate = 0.12m,
                        TotalExcludingVat = 0,
                        TotalIncludingVat = 0,
                        TotalVat = 0
                    }
                }
            };

            return transaction;
        }

        public Payment CreatePayment(PaymentService paymentService)
        {
            var payment = new Payment()
            {
                Id = this.UniqueString(),
                Priority = 1,
                PaymentOption = new ProviderOptionIdentifier(this.UniqueString(), "DirectPayment"),
                PaymentReference1 = this.UniqueString(),
                PaymentReference2 = this.UniqueString(),
                BillingAddress = new Litium.Sales.Address()
                {
                    FirstName = this.UniqueString(),
                    LastName = this.UniqueString(),
                    Address1 = "PaymentAddress"
                }
            };

            paymentService.Create(payment);
            AddDisposable(() => paymentService.Delete(payment));

            return payment;
        }

        public Sales.SalesOrder CreateSalesOrderWithPayment(TransactionType transactionType, TransactionResult transactionResult)
        {
            //Create order.
            var salesOrder = CreateSalesOrder();
            salesOrder.Rows.Add(new SalesOrderRow()
            {
                SystemId = Guid.NewGuid(),
                Description = this.UniqueString(),
                Quantity = 2,
                TotalIncludingVat = 55,
                TotalExcludingVat = 50,
                TotalVat = 5,
                UnitPriceIncludingVat = 55,
                UnitPriceExcludingVat = 50,
                VatRate = 10,
                OrderRowType = OrderRowType.Product,
                ProductType = ProductType.DigitalGoods,
                ArticleNumber = this.UniqueString()
            });
            _orderService.Create(salesOrder);
            AddDisposable(() => _orderService.Delete(salesOrder));
            //Create payment.
            var payment = CreatePayment(_paymentService);
            //Set Order payment links.
            salesOrder.OrderPaymentLinks.Add(new OrderPaymentLink(payment.SystemId));
            _orderService.Update(salesOrder);
            //Create transaction.
            var transaction = CreateTransaction(payment.SystemId, transactionType, transactionResult);
            _transactionService.Create(transaction);
            AddDisposable(() => _transactionService.Delete(transaction));

            return salesOrder;
        }

        public Sales.SalesOrder CreateSalesOrderForOrganization(TransactionType transactionType, TransactionResult transactionResult)
        {
            var orgFieldTemplate = new OrganizationFieldTemplate(this.UniqueString());
            _fieldTemplateService.Create(orgFieldTemplate);
            AddDisposable(() => _fieldTemplateService.Delete(orgFieldTemplate));

            var org = new Organization(orgFieldTemplate.SystemId, this.UniqueString());
            _organizationService.Create(org);
            AddDisposable(() => _organizationService.Delete(org));

            Person.OrganizationLinks = new List<PersonToOrganizationLink>()
            {
                new PersonToOrganizationLink(org.SystemId)
            };
            _personService.Update(Person);

            //Create order.
            var salesOrder = CreateSalesOrder();
            salesOrder.Rows.Add(new SalesOrderRow()
            {
                SystemId = Guid.NewGuid(),
                Description = this.UniqueString(),
                Quantity = 2,
                TotalIncludingVat = 55,
                TotalExcludingVat = 50,
                TotalVat = 5,
                UnitPriceIncludingVat = 55,
                UnitPriceExcludingVat = 50,
                VatRate = 10,
                OrderRowType = OrderRowType.Product,
                ProductType = ProductType.DigitalGoods,
                ArticleNumber = this.UniqueString()
            });
            _orderService.Create(salesOrder);
            AddDisposable(() => _orderService.Delete(salesOrder));
            //Create payment.
            var payment = CreatePayment(_paymentService);
            //Set Order payment links.
            salesOrder.OrderPaymentLinks.Add(new OrderPaymentLink(payment.SystemId));
            _orderService.Update(salesOrder);
            //Create transaction.
            var transaction = CreateTransaction(payment.SystemId, transactionType, transactionResult);
            _transactionService.Create(transaction);
            AddDisposable(() => _transactionService.Delete(transaction));

            return salesOrder;
        }

        public Sales.Shipment CreateShipment()
        {
            var shipment = new Sales.Shipment()
            {
                Id = this.UniqueString(),
                TrackingReference = this.UniqueString(),
                TrackingUrl = this.UniqueString(),
                DeliveryInstructions = this.UniqueString(),
                ShipmentDocuments = new Dictionary<string, string> { { "key", "value" } },
                ExpectedDeliveryDate = DateTimeOffset.Now,
                ReceiverAddress = new Litium.Sales.Address()
                {
                    FirstName = this.UniqueString(),
                    LastName = this.UniqueString(),
                    Address1 = "Receiver Address"
                },
                DeliveryCarrierId = this.UniqueString(),
                CurrencyCode = this.UniqueString(),
                AdditionalInfo = new Dictionary<string, object> { { "key", "value" } },
                ContentDescription = this.UniqueString(),
                DeliveryCarrierServiceCode = this.UniqueString(),
                ReceiverReference = this.UniqueString(),
                SenderReference = this.UniqueString(),
                ShippingMethod = ShippingMethod.RegisteredPost,
                ShipmentType= ShipmentType.Fulfillment,
                TmsShippingOptionReferenceId = this.UniqueString(),
#pragma warning disable CS0618 // Type or member is obsolete
                VatSummary = new Dictionary<decimal, decimal> { { 1, 1 } }
#pragma warning restore CS0618 // Type or member is obsolete
            };

            return shipment;
        }
    }
}
