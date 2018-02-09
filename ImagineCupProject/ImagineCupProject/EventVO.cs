namespace ImagineCupProject
{
    public class EventVO
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

        private string EventFirstQuestion = "Are you in the location right now?";
        private string EventSecondQuestion = "When did this happen?";
        private string EventThirdQuestion = "Does anybody injured in the accidents scene?";
        private string EventFourthQuestion = "Are you with the patient?";
        private string EventFifthQuestion = "Is the patient AWAKE & ABLE TO TALK to you?";
        private string EventSixthQuestion = "Is the patient BREATHING normal?";
        private string EventSeventhQuestion = "Is the patient BLEEDING";
        private string EventEighthQuestion = "Patient's AGE and SEX?";

        private bool EventFirstAnswer;
        private string EventSecondAnswer;
        private bool EventThirdAnswer;
        private bool EventFourthAnswer;
        private bool EventFifthAnswer;
        private bool EventSixthAnswer;
        private bool EventSeventhAnswer;
        private string EventEighthAnswer;

        public EventVO() { }

        public EventVO(string EventNumber, string EventOperator, string EventStartTime, string EventEndTime,
            string EventLocation, string EventPhoneNumber, string EventCallerName, string EventProblem, string EventCode, 
            bool EventFirstAnswer, string EventSecondAnswer, bool EventThirdAnswer, bool EventFourthAnswer,
            bool EventFifthAnswer,bool EventSixthAnswer, bool EventSeventhAnswer, string EventEighthAnswer)
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

            this.EventFirstAnswer = EventFirstAnswer;
            this.EventSecondAnswer = EventSecondAnswer;
            this.EventThirdAnswer = EventThirdAnswer;
            this.EventFourthAnswer = EventFourthAnswer;
            this.EventFifthAnswer = EventFifthAnswer;
            this.EventSixthAnswer = EventSixthAnswer;
            this.EventSeventhAnswer = EventSeventhAnswer;
            this.EventEighthAnswer = EventEighthAnswer;
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

        public string EventFirstQUESTION
        {
            get { return EventFirstQuestion; }
        }
        public string EventSecondQUESTION
        {
            get { return EventSecondQuestion; }
        }
        public string EventThirdQUESTION
        {
            get { return EventThirdQuestion; }
        }
        public string EventFourthUESTION
        {
            get { return EventFourthQuestion; }
        }
        public string EventFifthQUESTION
        {
            get { return EventFifthQuestion; }
        }
        public string EventSixthQUESTION
        {
            get { return EventSixthQuestion; }
        }
        public string EventSeventhQUESTION
        {
            get { return EventSeventhQuestion; }
        }
        public string EventEighthQUESTION
        {
            get { return EventEighthQuestion; }
        }

        public bool EventFirstANSWER
        {
            get { return EventFirstAnswer; }
            set { EventFirstAnswer = value; }
        }
        public string EventSecondANSWER
        {
            get { return EventSecondAnswer; }
            set { EventSecondAnswer = value; }
        }
        public bool EventThirdANSWER
        {
            get { return EventThirdAnswer; }
            set { EventThirdAnswer = value; }
        }
        public bool EventFourthANSWER
        {
            get { return EventFourthAnswer; }
            set { EventFourthAnswer = value; }
        }
        public bool EventFifthANSWER
        {
            get { return EventFifthAnswer; }
            set { EventFifthAnswer = value; }
        }
        public bool EventSixthANSWER
        {
            get { return EventSixthAnswer; }
            set { EventSixthAnswer = value; }
        }
        public bool EventSeventhANSWER
        {
            get { return EventSeventhAnswer; }
            set { EventSeventhAnswer = value; }
        }
        public string EventEighthANSWER
        {
            get { return EventEighthAnswer; }
            set { EventEighthAnswer = value; }
        }

    }
}
