using AutoMapper;
using LifelogBb.ApiDTOs.StrengthTrainings;
using LifelogBb.Interfaces;
using LifelogBb.Models.Entities;
using LifelogBb.Utilities;
using Microsoft.EntityFrameworkCore;

namespace LifelogBb.ApiServices
{
    public class StrengthTrainingsService : BaseCRUDService<StrengthTraining, StrengthTrainingInput, StrengthTrainingOutput>
    {
        public StrengthTrainingsService(IRepository<StrengthTraining> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        // A strength training entry carries the day it was trained, which can differ from the day it
        // was logged (e.g. retroactive entry, or a day plan worked through the next morning).
        protected override string DefaultSortOrder => $"{nameof(StrengthTraining.Date)}_desc";

        // The MCP tools call this service directly, where the input DTO's [Range] never runs and EF
        // does not check data annotations before saving. Both write paths are guarded here instead.
        public override async Task<StrengthTrainingOutput> Create(StrengthTrainingInput inputModel)
        {
            TrainingSetRules.ValidateDuration(inputModel.DurationSeconds);
            return await base.Create(inputModel);
        }

        public override async Task<StrengthTrainingOutput> Update(long id, StrengthTrainingInput inputModel)
        {
            TrainingSetRules.ValidateDuration(inputModel.DurationSeconds);
            return await base.Update(id, inputModel);
        }
    }
}
