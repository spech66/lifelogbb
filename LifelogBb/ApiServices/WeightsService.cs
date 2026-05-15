using AutoMapper;
using LifelogBb.ApiDTOs.Weights;
using LifelogBb.Interfaces;
using LifelogBb.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifelogBb.ApiServices
{
    public class WeightsService : BaseCRUDService<Weight, WeightInput, WeightOutput>
    {
        public WeightsService(IRepository<Weight> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        public override async Task<WeightOutput> Create(WeightInput inputModel)
        {
            var entry = _mapper.Map<Weight>(inputModel);
            var config = Config.GetConfig(_repository.Context);
            entry.Height = config.Height;
            CalculateBmi(entry, config);
            entry.SetCreateFields();

            _repository.Insert(entry);
            await _repository.Context.SaveChangesAsync();

            return _mapper.Map<WeightOutput>(entry);
        }

        public override async Task<WeightOutput> Update(long id, WeightInput inputModel)
        {
            var dbEntry = await _repository.Query.FirstOrDefaultAsync(m => m.Id == id);

            if (dbEntry == null || id != dbEntry.Id)
                throw new Exception("Invalid id");

            dbEntry = _mapper.Map<WeightInput, Weight>(inputModel, dbEntry);
            var config = Config.GetConfig(_repository.Context);
            dbEntry.Height = config.Height;
            CalculateBmi(dbEntry, config);
            dbEntry.SetUpdateFields();

            _repository.Update(dbEntry);
            await _repository.Context.SaveChangesAsync();
            return _mapper.Map<WeightOutput>(dbEntry);
        }

        private static void CalculateBmi(Weight weight, Config config)
        {
            if (config.UnitsType == Measurements.Metric)
            {
                weight.Bmi = (weight.BodyWeight * 1.0) / (((weight.Height * 0.01) * weight.Height) * 0.01);
            }
            else
            {
                weight.Bmi = weight.BodyWeight / (weight.Height * weight.Height) * 703.0;
            }
        }
    }
}
