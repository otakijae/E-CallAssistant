namespace ImagineCupProject
{
    class EventVO
    {
        private string EventNumber;
        private string EventOperator;
        private string EventStartTime;
        private string EventEndTime;
        private string EventLocation;
        private string EventPhoneNumber;
        private string EventCallerName;
        private string EventProblem;
        private string EventCode;

        public EventVO() { }

        public EventVO(string EventNumber, string EventOperator, string EventStartTime, string EventEndTime, string EventLocation, string EventPhoneNumber, string EventCallerName, string EventProblem, string EventCode)
        {
            this.EventNumber = EventNumber;
            this.EventOperator = EventOperator;
            this.EventStartTime = EventStartTime;
            this.EventEndTime = EventEndTime;
            this.EventLocation = EventLocation;
            this.EventPhoneNumber = EventPhoneNumber;
            this.EventCallerName = EventCallerName;
            this.EventProblem = EventProblem;
            this.EventCode = EventCode;
        }

        public string EventNUMBER
        {
            get { return EventNumber; }
            set { EventNumber = value; }
        }

        public string EventOPERATOR
        {
            get { return EventOperator; }
            set { EventOperator = value; }
        }

        public string EventSTARTTIME
        {
            get { return EventStartTime; }
            set { EventStartTime = value; }
        }

        public string EventENDTIME
        {
            get { return EventEndTime; }
            set { EventEndTime = value; }
        }

        public string EventLOCATION
        {
            get { return EventLocation; }
            set { EventLocation = value; }
        }

        public string EventPHONENUMBER
        {
            get { return EventPhoneNumber; }
            set { EventPhoneNumber = value; }
        }

        public string EventCALLERNAME
        {
            get { return EventCallerName; }
            set { EventCallerName = value; }
        }

        public string EventPROBLEM
        {
            get { return EventProblem; }
            set { EventProblem = value; }
        }

        public string EventCODE
        {
            get { return EventCode; }
            set { EventCode = value; }
        }

    }
}
