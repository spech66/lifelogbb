using AutoMapper;
using LifelogBb.ApiDTOs.StrengthTrainings;
using LifelogBb.Interfaces;
using LifelogBb.Models.Entities;
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
    }
}
