using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace SimulatorPlatform
{
    class CreateInstances
    {
        // Mediator
        private Mediator cMediator;
        private Colleague cColleague_Model;
        private Colleague cColleague_View;
        private Colleague cColleague_Controller;
        // Model
        private Factory_Communicator cFactory_Communicator;
        private Factory_Simulator cFactory_Simulator;
        private Parser cParser;
        // View
        private Form1 cForm1;
        private Factory_subForm cFactory_subForm;
        //Controller
        private Factory_Controller cFactory_Controller;
        public String commType
        {
            get; set;
        }
        public CreateInstances(Form1 _Form1)
        {
            cForm1 = _Form1;
        }
        public void createInstances()
        {
            setMediator();
            setModel();
            setView();
            setController();
        }
        private void setMediator()
        {
            cMediator = new Mediator();
            cColleague_Model = new Colleague();
            cColleague_View = new Colleague();
            cColleague_Controller = new Colleague();
            cMediator.registerColleague("MODEL", cColleague_View);
            cMediator.registerColleague("VIEW", cColleague_Controller);
            cMediator.registerColleague("CONTROLLER", cColleague_Model);
            cColleague_Model.setMediator(cMediator);
            cColleague_View.setMediator(cMediator);
            cColleague_Controller.setMediator(cMediator);
        }

        private void setModel()
        {
            // MODEL
            cFactory_Communicator = new Factory_Communicator();
            cFactory_Simulator = new Factory_Simulator();
            cParser = new Parser();
            IF_Simulator tmp_Simulator = null;
            IF_Communicator tmp_Communicator = cFactory_Communicator.getInstance(commType);
            tmp_Simulator = cFactory_Simulator.getInstance("SIMULATOR1");
            // Instances 등록
            tmp_Simulator.registerParser(cParser);
            tmp_Simulator.registerCommunicator(tmp_Communicator);
            tmp_Communicator.registerReceiveEventHandlers(tmp_Simulator.recvEventHadler);
            // Mediator 설정
            tmp_Simulator.registerPublishEvent(cColleague_Model.publishData);
            cColleague_Model.messageEventHandler += new Colleague.delegateColleagueMessageHandler(tmp_Simulator.handleData);
            tmp_Simulator = cFactory_Simulator.getInstance("SIMULATOR2");
            // Instances 등록
            tmp_Simulator.registerParser(cParser);
            tmp_Simulator.registerCommunicator(tmp_Communicator);
            tmp_Communicator.registerReceiveEventHandlers(tmp_Simulator.recvEventHadler);
            // Mediator 설정
            tmp_Simulator.registerPublishEvent(cColleague_Model.publishData);
            cColleague_Model.messageEventHandler += new Colleague.delegateColleagueMessageHandler(tmp_Simulator.handleData);
        }
        private void setView()
        {
            cFactory_subForm = new Factory_subForm();
            cForm1.setFactory_subForm(cFactory_subForm);
            // Mediator 설정
            cForm1.registerPublishEvent(cColleague_View.publishData);
            cColleague_View.messageEventHandler += new Colleague.delegateColleagueMessageHandler(cForm1.handleData);
            IF_DataHandler tmp_View = null;
            tmp_View = (IF_DataHandler)cFactory_subForm.getInstance("SUBFORM1");
            // Mediator 설정
            tmp_View.registerPublishEvent(cColleague_View.publishData);
            cColleague_View.messageEventHandler += new Colleague.delegateColleagueMessageHandler(tmp_View.handleData);
            tmp_View = (IF_DataHandler)cFactory_subForm.getInstance("SUBFORM2");
            // Mediator 설정
            tmp_View.registerPublishEvent(cColleague_View.publishData);
            cColleague_View.messageEventHandler += new Colleague.delegateColleagueMessageHandler(tmp_View.handleData);
        }
        private void setController()
        {
            // CONTROLLER
            cFactory_Controller = new Factory_Controller();
            IF_Controller tmp_Controller = null;
            tmp_Controller = cFactory_Controller.getInstance("COMMUNICATOR");
            // Mediator 설정
            tmp_Controller.registerPublishEvent(cColleague_Controller.publishData);
            cColleague_Controller.messageEventHandler += new Colleague.delegateColleagueMessageHandler(tmp_Controller.handleData);
            tmp_Controller = cFactory_Controller.getInstance("SIMULATOR");
            // Mediator 설정
            tmp_Controller.registerPublishEvent(cColleague_Controller.publishData);
            cColleague_Controller.messageEventHandler += new Colleague.delegateColleagueMessageHandler(tmp_Controller.handleData);
            tmp_Controller = cFactory_Controller.getInstance("PARSER");
            // Mediator 설정
            tmp_Controller.registerPublishEvent(cColleague_Controller.publishData);
            cColleague_Controller.messageEventHandler += new Colleague.delegateColleagueMessageHandler(tmp_Controller.handleData);
        }
    }
}