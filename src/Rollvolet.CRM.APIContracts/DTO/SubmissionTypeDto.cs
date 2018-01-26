using System;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class SubmissionTypeDto : Resource<SubmissionTypeDto.AttributesDto, EmptyRelationshipsDto>
    {
        public class AttributesDto {
            public string Name { get; set; }
            public int Order { get; set; }
        }
  }
}