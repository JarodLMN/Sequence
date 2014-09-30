﻿using System;

using System.Threading.Tasks;

namespace Instanda.Sequence.Azure
{
    public class AzureTableStateProvider : TableEntityRepository<SequenceTableEntity>, IStateProvider
    {
        private const string PartitionKey = "Sequences";

        public AzureTableStateProvider(string connectionStr, string tableName)
            : base(connectionStr, tableName, false)
        {
            
        }

        public async Task<SequenceKey> AddAsync(ISequence sequence)
        {
            var rowKey = Guid.NewGuid().ToString();

            var sequenceEntity = (SequenceTableEntity)sequence;

            sequenceEntity.RowKey = rowKey;
            sequenceEntity.PartitionKey = PartitionKey;

            await base.Add(sequenceEntity);

            return new SequenceKey { Value = rowKey };
        }

        public async Task<ISequence> GetAsync(SequenceKey sequenceKey)
        {
            var sequenceEntity = await base.Get(PartitionKey, sequenceKey.Value);

            return sequenceEntity;
        }

        public async  Task<bool> UpdateAsync(SequenceKey sequenceKey, ISequence sequence)
        {
            var sequenceEntity = (SequenceTableEntity)sequence;

            sequenceEntity.RowKey = sequenceKey.Value;
            sequenceEntity.PartitionKey = PartitionKey;

           var updatedSequenceEntity  = await UpdateAsync(sequenceEntity);

           return updatedSequenceEntity != null;
        }

        public async Task<ISequence> NewAsync()
        {
            var sequence =  new SequenceTableEntity();
            SetDefaultValues(sequence);
            return await Task.FromResult(sequence);
        }

        private void SetDefaultValues(ISequence sequence)
        {
            sequence.StartAt = 0;
            sequence.Increment = 1;
            sequence.MaxValue = long.MaxValue;
            sequence.MinValue = 0;
            sequence.Cycle = false;
            sequence.CurrentValue = 0;

        }

    }
}