using AutoMapper;
using LifelogBb.ApiDTOs.Weights;
using LifelogBb.Interfaces;
using LifelogBb.Models.Entities;
using LifelogBb.Utilities;
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
            entry.Bmi = BmiHelper.Calculate(entry.BodyWeight, entry.Height, config.UnitsType);
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
            dbEntry.Bmi = BmiHelper.Calculate(dbEntry.BodyWeight, dbEntry.Height, config.UnitsType);
            dbEntry.SetUpdateFields();

            _repository.Update(dbEntry);
            await _repository.Context.SaveChangesAsync();
            return _mapper.Map<WeightOutput>(dbEntry);
        }

    }
}
