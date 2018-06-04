/*
* CoolQ Demo for VC++ 
* Api Version 9
* Written by Coxxs & Thanks for the help of orzFly
*/

#include "stdafx.h"
#include "shellapi.h";
#include "string"
#include "cqp.h"
#include "appmain.h" //Ӧ��AppID����Ϣ������ȷ��д�������Q�����޷�����
#include "Flexlive.CQP.CQCProxy.h"


using namespace std;
using namespace FlexliveCQP;
using namespace System::Diagnostics;

int ac = -1;
bool enabled = false;

/*
* ����Ӧ�õ�ApiVer��Appid������󽫲������
*/
CQEVENT(const char*, AppInfo, 0)() 
{
	return CQAPPINFO;
}


/*
* ����Ӧ��AuthCode����Q��ȡӦ����Ϣ��������ܸ�Ӧ�ã���������������������AuthCode��
* ��Ҫ�ڱ��������������κδ��룬���ⷢ���쳣���������ִ�г�ʼ����������Startup�¼���ִ�У�Type=1001����
*/
CQEVENT(int32_t, Initialize, 4)(int32_t AuthCode) {
	ac = AuthCode;

	return 0;
}


/*
* Type=1001 ��Q����
* ���۱�Ӧ���Ƿ����ã������������ڿ�Q������ִ��һ�Σ���������ִ��Ӧ�ó�ʼ�����롣
* ��Ǳ�Ҫ����������������ش��ڡ���������Ӳ˵������û��ֶ��򿪴��ڣ�
*/
CQEVENT(int32_t, __eventStartup, 0)() {

	FlexliveCQP::CQCPlugin::Initialize(gcnew String(CQ_getAppDirectory(ac)));

	return 0;
}


/*
* Type=1002 ��Q�˳�
* ���۱�Ӧ���Ƿ����ã������������ڿ�Q�˳�ǰִ��һ�Σ���������ִ�в���رմ��롣
* ������������Ϻ󣬿�Q���ܿ�رգ��벻Ҫ��ͨ���̵߳ȷ�ʽִ���������롣
*/
CQEVENT(int32_t, __eventExit, 0)() {

	return 0;
}


/*
* Type=1003 Ӧ���ѱ�����
* ��Ӧ�ñ����ú󣬽��յ����¼���
* �����Q����ʱӦ���ѱ����ã�����_eventStartup(Type=1001,��Q����)�����ú󣬱�����Ҳ��������һ�Ρ�
* ��Ǳ�Ҫ����������������ش��ڡ���������Ӳ˵������û��ֶ��򿪴��ڣ�
*/
CQEVENT(int32_t, __eventEnable, 0)() {
	enabled = true;
	return 0;
}


/*
* Type=1004 Ӧ�ý���ͣ��
* ��Ӧ�ñ�ͣ��ǰ�����յ����¼���
* �����Q����ʱӦ���ѱ�ͣ�ã��򱾺���*����*�����á�
* ���۱�Ӧ���Ƿ����ã���Q�ر�ǰ��������*����*�����á�
*/
CQEVENT(int32_t, __eventDisable, 0)() {
	enabled = false;
	return 0;
}


/*
* Type=21 ˽����Ϣ
* subType �����ͣ�11/���Ժ��� 1/��������״̬ 2/����Ⱥ 3/����������
*/
CQEVENT(int32_t, __eventPrivateMsg, 24)(int32_t subType, int32_t sendTime, int64_t fromQQ, const char *msg, int32_t font) {
	
	//���Ҫ�ظ���Ϣ������ÿ�Q�������ͣ��������� return EVENT_BLOCK - �ضϱ�����Ϣ�����ټ�������  ע�⣺Ӧ�����ȼ�����Ϊ"���"(10000)ʱ������ʹ�ñ�����ֵ
	//������ظ���Ϣ������֮���Ӧ��/�������������� return EVENT_IGNORE - ���Ա�����Ϣ
	FlexliveCQP::CQCPlugin::CSharpTrans("PrivateMessage", subType, sendTime, 0, 0, fromQQ, "", 0, gcnew String(msg), font, "", "");

	return EVENT_IGNORE;
}


/*
* Type=2 Ⱥ��Ϣ
*/
CQEVENT(int32_t, __eventGroupMsg, 36)(int32_t subType, int32_t sendTime, int64_t fromGroup, int64_t fromQQ, const char *fromAnonymous, const char *msg, int32_t font) {

	FlexliveCQP::CQCPlugin::CSharpTrans("GroupMessage", subType, sendTime, fromGroup, 0, fromQQ, gcnew String(fromAnonymous), 0, gcnew String(msg), font, "", "");
	
	return EVENT_IGNORE; //���ڷ���ֵ˵��, ����_eventPrivateMsg������
}


/*
* Type=4 ��������Ϣ
*/
CQEVENT(int32_t, __eventDiscussMsg, 32)(int32_t subType, int32_t sendTime, int64_t fromDiscuss, int64_t fromQQ, const char *msg, int32_t font) {

	FlexliveCQP::CQCPlugin::CSharpTrans("DiscussMessage", subType, sendTime, 0, fromDiscuss, fromQQ, "", 0, gcnew String(msg), font, "", "");
	
	return EVENT_IGNORE; //���ڷ���ֵ˵��, ����_eventPrivateMsg������
}


///*
//* Type=11 Ⱥ�ļ��ϴ�
//* subType �����ͣ�1/��ȡ������Ա 2/�����ù���Ա
//*/
//CQEVENT(int32_t, __eventGroupUpload, 32)(int32_t subType, int32_t sendTime, int64_t fromGroup, int64_t fromQQ, const char *file) {
//
//	FlexliveCQP::CQCPlugin::CSharpTrans("GroupUpload", subType, sendTime, fromGroup, 0, fromQQ, "", 0, "", 0, "", gcnew String(file));
//
//	return EVENT_IGNORE; //���ڷ���ֵ˵��, ����_eventPrivateMsg������
//}

/*
* Type=101 Ⱥ�¼�-����Ա�䶯
* subType �����ͣ�1/��ȡ������Ա 2/�����ù���Ա
*/
CQEVENT(int32_t, __eventSystem_GroupAdmin, 24)(int32_t subType, int32_t sendTime, int64_t fromGroup, int64_t beingOperateQQ) {

	FlexliveCQP::CQCPlugin::CSharpTrans("GroupAdmin", subType, sendTime, fromGroup, 0, 0, "", beingOperateQQ, "", 0, "", "");
	
	return EVENT_IGNORE; //���ڷ���ֵ˵��, ����_eventPrivateMsg������
}


/*
* Type=102 Ⱥ�¼�-Ⱥ��Ա����
* subType �����ͣ�1/ȺԱ�뿪 2/ȺԱ���� 3/�Լ�(����¼��)����
* fromQQ ������QQ(��subTypeΪ2��3ʱ����)
* beingOperateQQ ������QQ
*/
CQEVENT(int32_t, __eventSystem_GroupMemberDecrease, 32)(int32_t subType, int32_t sendTime, int64_t fromGroup, int64_t fromQQ, int64_t beingOperateQQ) {

	FlexliveCQP::CQCPlugin::CSharpTrans("GroupMemberDecrease", subType, sendTime, fromGroup, 0, fromQQ, "", beingOperateQQ, "", 0, "", "");
	
	return EVENT_IGNORE; //���ڷ���ֵ˵��, ����_eventPrivateMsg������
}


/*
* Type=103 Ⱥ�¼�-Ⱥ��Ա����
* subType �����ͣ�1/����Ա��ͬ�� 2/����Ա����
* fromQQ ������QQ(������ԱQQ)
* beingOperateQQ ������QQ(����Ⱥ��QQ)
*/
CQEVENT(int32_t, __eventSystem_GroupMemberIncrease, 32)(int32_t subType, int32_t sendTime, int64_t fromGroup, int64_t fromQQ, int64_t beingOperateQQ) {

	FlexliveCQP::CQCPlugin::CSharpTrans("GroupMemberIncrease", subType, sendTime, fromGroup, 0, fromQQ, "", beingOperateQQ, "", 0, "", "");
	
	return EVENT_IGNORE; //���ڷ���ֵ˵��, ����_eventPrivateMsg������
}


/*
* Type=201 �����¼�-���������
*/
CQEVENT(int32_t, __eventFriend_Add, 16)(int32_t subType, int32_t sendTime, int64_t fromQQ) {

	FlexliveCQP::CQCPlugin::CSharpTrans("FriendAdded", subType, sendTime, 0, 0, fromQQ, "", 0, "", 0, "", "");
	
	return EVENT_IGNORE; //���ڷ���ֵ˵��, ����_eventPrivateMsg������
}


/*
* Type=301 ����-�������
* msg ����
* responseFlag ������ʶ(����������)
*/
CQEVENT(int32_t, __eventRequest_AddFriend, 24)(int32_t subType, int32_t sendTime, int64_t fromQQ, const char *msg, const char *responseFlag) {

	//CQ_setFriendAddRequest(ac, responseFlag, REQUEST_ALLOW, "");
	FlexliveCQP::CQCPlugin::CSharpTrans("RequestAddFriend", subType, sendTime, 0, 0, fromQQ, "", 0, gcnew String(msg), 0, gcnew String(responseFlag), "");
	
	return EVENT_IGNORE; //���ڷ���ֵ˵��, ����_eventPrivateMsg������
}


/*
* Type=302 ����-Ⱥ���
* subType �����ͣ�1/����������Ⱥ 2/�Լ�(����¼��)������Ⱥ
* msg ����
* responseFlag ������ʶ(����������)
*/
CQEVENT(int32_t, __eventRequest_AddGroup, 32)(int32_t subType, int32_t sendTime, int64_t fromGroup, int64_t fromQQ, const char *msg, const char *responseFlag) {

	//if (subType == 1) {
	//	CQ_setGroupAddRequestV2(ac, responseFlag, REQUEST_GROUPADD, REQUEST_ALLOW, "");
	//} else if (subType == 2) {
	//	CQ_setGroupAddRequestV2(ac, responseFlag, REQUEST_GROUPINVITE, REQUEST_ALLOW, "");
	//}
	FlexliveCQP::CQCPlugin::CSharpTrans("RequestAddGroup", subType, sendTime, fromGroup, 0, fromQQ, "", 0, gcnew String(msg), 0, gcnew String(responseFlag), "");
	
	return EVENT_IGNORE; //���ڷ���ֵ˵��, ����_eventPrivateMsg������
}

/*
* �˵������� .json �ļ������ò˵���Ŀ��������
* �����ʹ�ò˵������� .json ���˴�ɾ�����ò˵�
*/
CQEVENT(int32_t, __menuA, 0)() {
	
	ShellExecute(NULL, L"open", L"https://cqp.cc/t/29261", NULL, NULL, SW_HIDE);

	return 0;
}

CQEVENT(int32_t, __menuB, 0)() {
	//MessageBoxA(NULL, "����menuA�����������봰�ڣ����߽�������������", "", 0);

	String^ appDirectory = Path::Combine(gcnew String(CQ_getAppDirectory(ac)), "..\\");
	appDirectory = Path::Combine(appDirectory, "..\\");
	appDirectory = Path::Combine(appDirectory, "Flexlive.CQP.CSharpProxy.exe");
	appDirectory = appDirectory + " CLR";

	char* ch2 = (char*)(void*)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(appDirectory);

	WinExec(ch2, SW_SHOW);

	return 0;
}

__declspec(dllexport) int GetAuthCode()
{
	return ac;
}



