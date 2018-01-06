using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using StockMarket.Models;

namespace StockMarket.Controllers.API {
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CompaniesController : ODataController {
        private StockDataEntities db = new StockDataEntities();

        // GET odata/Companies
        [EnableQuery]
        public IQueryable<Company> GetCompanies() {
            return db.Companies;
        }

        // GET odata/Companies(5)
        [EnableQuery]
        public SingleResult<Company> GetCompany([FromODataUri] int key) {
            return SingleResult.Create(db.Companies.Where(company => company.ID == key));
        }

        // PUT odata/Companies(5)
        public IHttpActionResult Put([FromODataUri] int key, Company company) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (key != company.ID) {
                return BadRequest();
            }

            db.Entry(company).State = EntityState.Modified;

            try {
                db.SaveChanges();
            } catch (DbUpdateConcurrencyException) {
                if (!CompanyExists(key)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return Updated(company);
        }

        // POST odata/Companies
        public IHttpActionResult Post(Company company) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            db.Companies.Add(company);

            try {
                db.SaveChanges();
            } catch (DbUpdateException) {
                if (CompanyExists(company.ID)) {
                    return Conflict();
                } else {
                    throw;
                }
            }

            return Created(company);
        }

        // PATCH odata/Companies(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Company> patch) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            Company company = db.Companies.Find(key);
            if (company == null) {
                return NotFound();
            }

            patch.Patch(company);

            try {
                db.SaveChanges();
            } catch (DbUpdateConcurrencyException) {
                if (!CompanyExists(key)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return Updated(company);
        }

        // DELETE odata/Companies(5)
        public IHttpActionResult Delete([FromODataUri] int key) {
            Company company = db.Companies.Find(key);
            if (company == null) {
                return NotFound();
            }

            db.Companies.Remove(company);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET odata/Companies(5)/StockDatas
        [EnableQuery]
        public IQueryable<StockData> GetStockDatas([FromODataUri] int key) {
            return db.Companies.Where(m => m.ID == key).SelectMany(m => m.StockDatas);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CompanyExists(int key) {
            return db.Companies.Count(e => e.ID == key) > 0;
        }
    }
}
