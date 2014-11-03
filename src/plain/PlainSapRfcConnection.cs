﻿using SAP.Middleware.Connector;
using System;

namespace SharpSapRfc.Plain
{
    public class PlainSapRfcConnection : SapRfcConnection
    {
        public RfcRepository Repository 
        {
            get {
                this.EnsureConnectionIsOpen();
                return this._repository;
            }
        }

        public RfcDestination Destination
        {
            get
            {
                this.EnsureConnectionIsOpen();
                return this._destination;
            }
        }

        public PlainSapRfcConnection(string destinationName)
        {
            this._destinationName = destinationName;
            this._structureMapper = new PlainRfcStructureMapper(new PlainRfcValueMapper());
        }

        private void EnsureConnectionIsOpen()
        {
            if (!_isOpen)
            {
                try { 
                    this._destination = RfcDestinationManager.GetDestination(_destinationName);
                    this._repository = this._destination.Repository;
                    this._isOpen = true;
                }
                catch (Exception ex)
                {
                    throw new SharpRfcException("Could not connect to SAP.", ex);
                }
            }
        }

        public override RfcPreparedFunction PrepareFunction(string functionName)
        {
            EnsureConnectionIsOpen();
            return new PlainRfcPreparedFunction(functionName, this._structureMapper, this.Repository, this.Destination);
        }

        public override void Dispose()
        {
            
        }

        private string _destinationName;
        private bool _isOpen = false;
        private RfcRepository _repository;
        private RfcDestination _destination;
        private PlainRfcStructureMapper _structureMapper;

        protected override RfcStructureMapper GetStructureMapper()
        {
            return this._structureMapper;
        }

        public override void SetTimeout(int timeout)
        {
            //there is no timeout for plain rfc
        }
    }
}
